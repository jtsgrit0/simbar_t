using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Random = UnityEngine.Random;

public class CustomerSpawner : MonoBehaviour
{
    private static Shader GetSafeShader()
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        return shader;
    }

    private static Material CreateSafeMaterial(Color color)
    {
        var material = new Material(GetSafeShader());
        material.color = color;
        return material;
    }

    public Transform spawnPoint;
    public Transform exitPoint;
    public List<Table> tables = new List<Table>();
    public List<Customer> customerPrefabs = new List<Customer>();
    public float spawnInterval = 15f;
    public float spawnVariance = 5f;

    private float spawnTimer;
    private List<Customer> activeCustomers = new List<Customer>();

    private void Awake()
    {
        if (spawnPoint == null)
        {
            var point = new GameObject("SpawnPoint");
            point.transform.SetParent(transform);
            point.transform.position = new Vector3(-9f, 0f, 8f);
            spawnPoint = point.transform;
        }

        if (exitPoint == null)
        {
            var exit = new GameObject("ExitPoint");
            exit.transform.SetParent(transform);
            exit.transform.position = new Vector3(12f, 0f, 8f);
            exitPoint = exit.transform;
        }

        EnsureNavMeshSurface();
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.isOpen) return;

        if (tables.Count == 0)
        {
            tables.AddRange(FindObjectsOfType<Table>());
        }

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnCustomer();
            spawnTimer = spawnInterval + Random.Range(-spawnVariance, spawnVariance);
        }
    }

    private void SpawnCustomer()
    {
        Table freeTable = tables.Find(t => t != null && !t.isOccupied);
        if (freeTable == null) return;

        Customer customer = CreateRuntimeCustomer();
        if (customer == null) return;

        activeCustomers.Add(customer);
        customer.EnterPub(spawnPoint, freeTable);
        freeTable.isOccupied = true;
        if (TryGetComponent<OrderManager>(out var orderManager))
        {
            orderManager.RegisterCustomer(customer);
        }
    }

    private Customer CreateRuntimeCustomer()
    {
        if (customerPrefabs.Count > 0)
        {
            Customer selected = customerPrefabs[Random.Range(0, customerPrefabs.Count)];
            if (selected != null)
            {
                return Instantiate(selected, spawnPoint.position, Quaternion.identity);
            }
        }

        if (spawnPoint == null)
        {
            return null;
        }

        if (!HasValidNavMeshAt(spawnPoint.position))
        {
            EnsureNavMeshSurface();
            if (!HasValidNavMeshAt(spawnPoint.position))
            {
                Debug.LogWarning("CustomerSpawner: no valid NavMesh is baked for this scene. Customer spawn skipped.");
                return null;
            }
        }

        GameObject customerGO = LoadWalkingCustomerPrefab();
        if (customerGO == null)
        {
            customerGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            customerGO.name = "Customer";
            customerGO.transform.position = spawnPoint.position;
            customerGO.transform.localScale = new Vector3(0.7f, 0.9f, 0.7f);

            var renderer = customerGO.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = CreateSafeMaterial(new Color(Random.Range(0.2f, 0.9f), Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.9f)));
            }
        }
        else
        {
            customerGO = Instantiate(customerGO, spawnPoint.position, Quaternion.identity);
            customerGO.name = "Customer_Walking";
        }

        try
        {
            if (customerGO.GetComponent<NavMeshAgent>() == null)
            {
                customerGO.AddComponent<NavMeshAgent>();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("CustomerSpawner: failed to create NavMeshAgent: " + e.Message);
            Destroy(customerGO);
            return null;
        }

        // Check if Animation component already exists to avoid duplicates
        Animation animation = customerGO.GetComponent<Animation>();
        if (animation == null)
        {
            animation = customerGO.AddComponent<Animation>();
        }
        // Load walking animation clip from project assets (simplest way to get the clip)
#if UNITY_EDITOR
        AnimationClip walkingClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Models/walking.fbx");
        if (walkingClip != null)
        {
            walkingClip.wrapMode = WrapMode.Loop;
            animation.AddClip(walkingClip, "Walking");
            animation.Play("Walking");
        }
#endif

        var customer = customerGO.GetComponent<Customer>();
        if (customer == null)
        {
            customer = customerGO.AddComponent<Customer>();
        }

        customer.customerName = "Guest " + Random.Range(1, 99);
        return customer;
    }

    private static GameObject LoadWalkingCustomerPrefab()
    {
#if UNITY_EDITOR
        var walking = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Model/walking.fbx");
        if (walking != null)
        {
            return walking;
        }
#endif

        return null;
    }

    private static bool HasValidNavMeshAt(Vector3 worldPosition)
    {
        var triangles = NavMesh.CalculateTriangulation();
        if (triangles.indices == null || triangles.indices.Length == 0)
        {
            return false;
        }

        return NavMesh.SamplePosition(worldPosition, out _, 2f, NavMesh.AllAreas);
    }

    private static void EnsureNavMeshSurface()
    {
        if (HasValidNavMeshAt(Vector3.zero))
        {
            return;
        }

        GameObject navRoot = GameObject.Find("RuntimeNavMeshRoot");
        if (navRoot == null)
        {
            navRoot = new GameObject("RuntimeNavMeshRoot");
        }

        GameObject floor = navRoot.transform.Find("NavMeshFloor")?.gameObject;
        if (floor == null)
        {
            floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "NavMeshFloor";
            floor.transform.SetParent(navRoot.transform);
            floor.transform.position = new Vector3(0f, -0.5f, 0f);
            floor.transform.localScale = new Vector3(24f, 1f, 18f);

            var box = floor.GetComponent<BoxCollider>();
            if (box == null)
            {
                floor.AddComponent<BoxCollider>();
            }

            var renderer = floor.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }

        var surface = floor.GetComponent<NavMeshSurface>();
        if (surface == null)
        {
            surface = floor.AddComponent<NavMeshSurface>();
        }

        surface.collectObjects = CollectObjects.Children;
        surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        surface.defaultArea = 0;
        surface.overrideTileSize = false;
        surface.overrideVoxelSize = false;
        surface.BuildNavMesh();
    }

    public void OpenBusiness()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isOpen = true;
        }
        spawnTimer = 2f;
    }

    public void CloseBusiness()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isOpen = false;
        }
        foreach (var customer in activeCustomers)
        {
            if (customer != null && customer.currentState != Customer.CustomerState.Exited)
            {
                customer.Leave(false);
            }
        }
    }
}
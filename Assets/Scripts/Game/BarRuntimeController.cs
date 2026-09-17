using UnityEngine;

public class BarRuntimeController : MonoBehaviour
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

    private void Start()
    {
        EnsureRuntimeObjects();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isOpen = true;
        }
    }

    private void EnsureRuntimeObjects()
    {
        if (FindObjectOfType<BarSceneBuilder>() == null)
        {
            var builder = new GameObject("BarSceneBuilder");
            builder.AddComponent<BarSceneBuilder>();
        }

        if (FindObjectOfType<CustomerSpawner>() == null)
        {
            var spawner = new GameObject("CustomerSpawner");
            spawner.AddComponent<CustomerSpawner>();
        }

        if (FindObjectOfType<BarUIBuilder>() == null)
        {
            var ui = new GameObject("BarUIBuilder");
            ui.AddComponent<BarUIBuilder>();
        }

        if (FindObjectOfType<CozyAmbientAudio>() == null)
        {
            var audio = new GameObject("CozyAmbientAudio");
            audio.AddComponent<CozyAmbientAudio>();
        }

        if (FindObjectOfType<PlayerController>() == null)
        {
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.position = new Vector3(0f, 1f, 3f);
            player.transform.localScale = new Vector3(0.8f, 0.9f, 0.8f);
            player.AddComponent<CharacterController>();
            player.AddComponent<PlayerController>();

            var renderer = player.GetComponent<Renderer>();
            renderer.material = CreateSafeMaterial(new Color(0.2f, 0.75f, 1f, 1f));
        }

        if (Camera.main == null)
        {
            var camera = new GameObject("Main Camera");
            camera.tag = "MainCamera";
            camera.AddComponent<Camera>();
            camera.transform.position = new Vector3(0f, 8f, -10f);
            camera.transform.rotation = Quaternion.Euler(25f, 0f, 0f);
        }
    }
}

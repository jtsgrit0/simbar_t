using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BarUIBuilder : MonoBehaviour
{
    public List<Sprite> artSprites;

    private const float ReferenceWidth = 1408f;
    private const float ReferenceHeight = 768f;

    private Canvas canvas;
    private Text cashText;
    private Text fansText;
    private Text timeText;

    private void Start()
    {
        BuildUI();
    }

    public void BuildUI()
    {
        if (canvas != null)
            return;

        GameObject canvasGO = new GameObject("BarCanvas");
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 120;
        canvas.pixelPerfect = true;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 1f; // 1 = Match Height

        canvasGO.AddComponent<GraphicRaycaster>();

        CreateReferencePreviewPanels();
        UpdateStatusText();
    }

    private void CreateReferencePreviewPanels()
    {
        // Keep the layout in the reference 1408x768 coordinate space.
        // These values are already measured in screen-space pixels for the composition,
        // so they should be used directly rather than being re-scaled with Screen.width/height.
        // Treat the reference image as a top-left anchored layout, matching the provided mockup.
        CreateArtPanel("Preview_Setup", "setup", new Vector2(12f, 10f), Vector2.zero);

        CreateArtPanel("Preview_TopCash", "cash", new Vector2(818f, 12f), Vector2.zero);
        CreateArtPanel("Preview_TopFans", "fans", new Vector2(998f, 12f), Vector2.zero);
        CreateArtPanel("Preview_TopTime", "time", new Vector2(1178f, 12f), Vector2.zero);

        // Reposition panels to the right side
        GameObject playlistPanel = CreateArtPanel("Preview_Playlist", "playlist", new Vector2(1070f, 400f), Vector2.zero);
        RectTransform playlistRT = playlistPanel.GetComponent<RectTransform>();
        playlistRT.anchorMin = new Vector2(1f, 1f);
        playlistRT.anchorMax = new Vector2(1f, 1f);
        playlistRT.pivot = new Vector2(1f, 1f);
        playlistRT.anchoredPosition = new Vector2(-20f, -400f); // Adjust position from right

        CreateArtPanel("Preview_BottomLeft", "staff", new Vector2(19f, 576f), Vector2.zero);

        // --- Reposition panels to the bottom center ---
        float referenceCenter = ReferenceWidth / 2f;

        // Upgrades Panel
        GameObject upgradesPanel = CreateArtPanel("Preview_Upgrades", "upgrades", new Vector2(450f, 775f), Vector2.zero);
        RectTransform upgradesRT = upgradesPanel.GetComponent<RectTransform>();
        upgradesRT.anchorMin = new Vector2(0.5f, 0f); // Bottom-center anchor
        upgradesRT.anchorMax = new Vector2(0.5f, 0f);
        upgradesRT.pivot = new Vector2(0.5f, 0f); // Pivot at bottom-center
        upgradesRT.anchoredPosition = new Vector2(450f - referenceCenter, 20f); // Position relative to center, 20px from bottom

        // Guest Panel
        GameObject guestPanel = CreateArtPanel("Preview_Guest", "guest", new Vector2(600f, 775f), Vector2.zero);
        RectTransform guestRT = guestPanel.GetComponent<RectTransform>();
        guestRT.anchorMin = new Vector2(0.5f, 0f);
        guestRT.anchorMax = new Vector2(0.5f, 0f);
        guestRT.pivot = new Vector2(0.5f, 0f);
        guestRT.anchoredPosition = new Vector2(600f - referenceCenter, 20f);

        // Events Panel
        GameObject eventsPanel = CreateArtPanel("Preview_Events", "events", new Vector2(750f, 775f), Vector2.zero);
        RectTransform eventsRT = eventsPanel.GetComponent<RectTransform>();
        eventsRT.anchorMin = new Vector2(0.5f, 0f);
        eventsRT.anchorMax = new Vector2(0.5f, 0f);
        eventsRT.pivot = new Vector2(0.5f, 0f);
        eventsRT.anchoredPosition = new Vector2(750f - referenceCenter, 20f);

        CreateArtPanel("Preview_Profit", "profit", new Vector2(200f, 680f), Vector2.zero);


        GameObject satisfactionPanel = CreateArtPanel("Preview_BottomRightGuest", "guestsati", new Vector2(1072f, 650f), Vector2.zero);
        RectTransform satisfactionRT = satisfactionPanel.GetComponent<RectTransform>();
        satisfactionRT.anchorMin = new Vector2(1f, 1f);
        satisfactionRT.anchorMax = new Vector2(1f, 1f);
        satisfactionRT.pivot = new Vector2(1f, 1f);
        satisfactionRT.anchoredPosition = new Vector2(-20f, -650f); // Adjust position from right

        GameObject performancePanel = CreateArtPanel("Preview_BottomRightPerformance", "perrform", new Vector2(1200f, 650f), Vector2.zero);
        RectTransform performanceRT = performancePanel.GetComponent<RectTransform>();
        performanceRT.anchorMin = new Vector2(1f, 1f);
        performanceRT.anchorMax = new Vector2(1f, 1f);
        performanceRT.pivot = new Vector2(1f, 1f);
        performanceRT.anchoredPosition = new Vector2(-150f, -650f); // Adjust position from right

        CreateArtPanel("Preview_Minimap", "minimap", new Vector2(20f, 620f), Vector2.zero);
    }

    private Sprite LoadSpriteFromArtImg(string fileName)
    {
        if (artSprites != null)
        {
            foreach (Sprite sprite in artSprites)
            {
                if (sprite != null && sprite.name == fileName)
                {
                    return sprite;
                }
            }
        }
        return null;
    }

    private GameObject CreateArtPanel(string name, string fileName, Vector2 anchoredPosition, Vector2 size)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(canvas.transform, false);

        Image img = panel.AddComponent<Image>();
        img.raycastTarget = false;
        img.type = Image.Type.Simple;
        img.preserveAspect = true;
        img.pixelsPerUnitMultiplier = 1f;
        img.color = new Color(1f, 1f, 1f, 1f);

        Sprite sprite = LoadSpriteFromArtImg(fileName);
        if (sprite != null)
        {
            img.sprite = sprite;
            img.SetNativeSize();
            size = new Vector2(sprite.rect.width, sprite.rect.height);
            img.color = Color.white;
        }
        else
        {
            img.color = new Color(0.35f, 0.35f, 0.45f, 0.85f);
            Debug.LogWarning("BarUIBuilder could not load sprite: " + fileName + ".png. Check if it is assigned in the Inspector.");
        }

        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);

        if (size != Vector2.zero)
            rt.sizeDelta = size;

        // Convert reference image coordinates to Unity screen-space coordinates with origin at top-left.
        rt.anchoredPosition = new Vector2(anchoredPosition.x, -anchoredPosition.y);

        return panel;
    }

    private void CreateTopStatusBar()
    {
        GameObject gear = new GameObject("SettingsButton");
        gear.transform.SetParent(canvas.transform, false);
        Image gearImg = gear.AddComponent<Image>();
        gearImg.raycastTarget = false;
        gearImg.color = new Color(0.18f, 0.18f, 0.24f, 0.9f);
        RectTransform gearRT = gear.GetComponent<RectTransform>();
        gearRT.anchorMin = new Vector2(0f, 0f);
        gearRT.anchorMax = new Vector2(0f, 0f);
        gearRT.pivot = new Vector2(0f, 0f);
        gearRT.sizeDelta = new Vector2(42f, 42f);
        gearRT.anchoredPosition = new Vector2(30f, 1000f);

        Image gearIcon = CreateImageIcon("GearIcon", gear.transform, "cog", new Vector2(0.5f, 0.5f), new Vector2(1f, 1f), Vector2.zero);
        gearIcon.color = new Color(1f, 1f, 1f, 0.92f);

        CreateArtPanel("CashPanel", "cash", new Vector2(1120f, 1010f), new Vector2(160f, 62f));
        CreateArtPanel("FansPanel", "fans", new Vector2(1285f, 1010f), new Vector2(152f, 62f));
        CreateArtPanel("TimePanel", "time", new Vector2(1445f, 1010f), new Vector2(118f, 62f));
    }

    private void CreatePlaylistPanel()
    {
        CreateArtPanel("PlaylistPanel", "playlist", new Vector2(1385f, 470f), new Vector2(270f, 200f));
    }

    private void CreateBottomLeftPanel()
    {
        CreateArtPanel("BottomLeftPanel", "staff", new Vector2(180f, 120f), new Vector2(332f, 162f));
    }

    private void CreateBottomRightCards()
    {
        CreateArtPanel("GuestCard", "guestsati", new Vector2(1240f, 140f), new Vector2(148f, 82f));
        CreateArtPanel("PerformanceCard", "perrform", new Vector2(1415f, 140f), new Vector2(148f, 82f));
    }

    private Text CreateStatusPill(string name, string value, Vector2 anchor, Vector2 size, Color color, int fontSize, TextAnchor alignment, bool showBorder, string iconName = null)
    {
        GameObject pill = new GameObject(name);
        pill.transform.SetParent(canvas.transform, false);

        Image bg = pill.AddComponent<Image>();
        bg.color = color;

        string panelSpriteName = "";
        if (name == "CashText") panelSpriteName = "cash_panel";
        else if (name == "FansText") panelSpriteName = "fans_panel";
        else if (name == "TimeText") panelSpriteName = "time_panel";

        if (!string.IsNullOrEmpty(panelSpriteName))
        {
            Sprite panelSprite = Resources.Load<Sprite>("UI/PanelCrops/" + panelSpriteName);
            if (panelSprite != null)
            {
                bg.sprite = panelSprite;
                bg.type = Image.Type.Simple;
                bg.color = new Color(1f, 1f, 1f, 1f);
            }
        }

        RectTransform rt = pill.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor + size;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        if (showBorder)
        {
            var outline = pill.AddComponent<Outline>();
            outline.effectColor = new Color(0.18f, 0.18f, 0.24f, 0.8f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);
        }

        if (!string.IsNullOrEmpty(iconName))
        {
            float leftOffset = iconName == "coin" ? -10f : iconName == "clock" ? -4f : 0f;
            float iconWidth = iconName == "users" ? 0.17f : 0.14f;
            float iconHeight = iconName == "clock" ? 0.44f : 0.52f;
            Image icon = CreateImageIcon(name + "Icon", pill.transform, iconName, new Vector2(0.10f, 0.5f), new Vector2(iconWidth, iconHeight), new Vector2(leftOffset, -2f));
            icon.color = new Color(1f, 1f, 1f, 0.98f);
            icon.preserveAspect = true;
        }

        return CreateText(name + "Label", pill.transform, value, new Vector2(0.5f, 0.5f), new Vector2(1f, 1f), alignment, fontSize);
    }

    private Image CreateImageIcon(string name, Transform parent, string resourceName, Vector2 anchor, Vector2 size, Vector2 anchoredPosition)
    {
        GameObject iconGO = new GameObject(name);
        iconGO.transform.SetParent(parent, false);

        Image iconImage = iconGO.AddComponent<Image>();
        iconImage.raycastTarget = false;
        iconImage.preserveAspect = true;
        iconImage.type = Image.Type.Simple;

        Sprite sprite = null;

        sprite = Resources.Load<Sprite>("UI/IconsPng/" + resourceName);
        if (sprite == null)
        {
            sprite = Resources.Load<Sprite>("UI/Icons/" + resourceName);
        }

        if (sprite == null)
        {
            Texture2D tex = Resources.Load<Texture2D>("UI/IconsPng/" + resourceName);
            if (tex != null)
            {
                tex.filterMode = FilterMode.Point;
                // Unity 2022+에서 alphaIsTransparency 제거됨 - Read/Write 활성화로 대체
                if (!tex.isReadable)
                {
                    Texture2D readableTex = new Texture2D(tex.width, tex.height, tex.format, tex.mipmapCount, false);
                    Graphics.CopyTexture(tex, readableTex);
                    tex = readableTex;
                }
                sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                sprite.name = resourceName;
            }
        }

        if (sprite == null)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("UI/IconsPng");
            if (sprites != null)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    if (sprites[i] != null && sprites[i].name == resourceName)
                    {
                        sprite = sprites[i];
                        break;
                    }
                }
            }
        }

        if (sprite == null)
        {
            Sprite[] iconSprites = Resources.LoadAll<Sprite>("UI/Icons");
            if (iconSprites != null)
            {
                for (int i = 0; i < iconSprites.Length; i++)
                {
                    if (iconSprites[i] != null && iconSprites[i].name == resourceName)
                    {
                        sprite = iconSprites[i];
                        break;
                    }
                }
            }
        }

        if (sprite != null)
        {
            iconImage.sprite = sprite;
            iconImage.color = new Color(1f, 1f, 1f, 1f);
        }

        RectTransform rt = iconGO.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor + size;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.anchoredPosition = anchoredPosition;
        rt.pivot = new Vector2(0.5f, 0.5f);

        return iconImage;
    }

    private void CreatePlaylistRowText(Transform parent, string name, string text, Vector2 anchor, Vector2 size, bool active)
    {
        Text rowText = CreateText(name, parent, text, anchor, size, TextAnchor.MiddleLeft, 12);
        rowText.color = active ? new Color(0.31f, 0.86f, 1f, 1f) : new Color(0.9f, 0.9f, 0.9f, 0.88f);
        rowText.fontStyle = active ? FontStyle.Bold : FontStyle.Normal;
    }

    private void CreateBottomActionButton(string label, Vector2 anchor)
    {
        GameObject btn = new GameObject(label + "Button");
        btn.transform.SetParent(canvas.transform, false);

        Image img = btn.AddComponent<Image>();
        img.color = new Color(0.18f, 0.18f, 0.22f, 0.9f);

        RectTransform rt = btn.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor + new Vector2(0.18f, 0.08f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        CreateText(label + "Text", btn.transform, label, new Vector2(0.5f, 0.5f), new Vector2(1f, 1f), TextAnchor.MiddleCenter, 16);
    }

    private Text CreateText(string name, Transform parent, string text, Vector2 anchor, Vector2 size, TextAnchor alignment, int fontSize)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Text txt = go.AddComponent<Text>();
        txt.text = text;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = fontSize;
        txt.alignment = alignment;
        txt.color = new Color(0.96f, 0.94f, 0.88f, 1f);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor + size;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        return txt;
    }

    private void UpdateStatusText()
    {
        if (GameManager.Instance == null)
            return;

        if (cashText != null)
            cashText.text = "Cash\n$" + GameManager.Instance.money.ToString("N0");

        if (fansText != null)
            fansText.text = "Fans\n" + GameManager.Instance.reputation;

        if (timeText != null)
            timeText.text = "Time\n21:30";
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Codes.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;

public class GazeLogger : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Canvas sphereHeatmapCanvas;
    [SerializeField] private Image heatmapImage;

    private const int CANVAS_X_MIN = 0;
    private const int CANVAS_X_MAX = 512;
    
    private const int CANVAS_Y_MIN = -300;
    private const int CANVAS_Y_MAX = -60;
    
    private const int HEATMAP_RADIUS = 32;

    private Sprite _heatmapSprite;

    private bool _isLogging;
    private List<Vector3> _gazeLog = new()
    {
        new Vector3(1,0,0),
        new Vector3(0,0,1),
    };

    void Update()
    {
        if (_isLogging)
        {
            TrackGazeData();
        }
    }

    private void Start()
    {
        RenderHeatmap();
    }

    private void LoadGazeLog()
    {
        // TODO:
    }

    public void RenderHeatmap()
    {
        Debug.Log(_gazeLog.Count);
        
        var sampleSize = _gazeLog.Count;
        var width = CANVAS_X_MAX - CANVAS_X_MIN;
        var height = CANVAS_Y_MAX - CANVAS_Y_MIN;
        var r = (CANVAS_X_MAX - CANVAS_X_MIN) / (Math.PI * 2);
        float[,] heatmapData = new float[width, height];
        Texture2D heatmapTexture = new Texture2D(width, height);

        foreach (Vector3 dir in _gazeLog)
        {
            var phi = Math.Atan(dir.x / dir.z) + Math.PI;
            var rSinTheta = Math.Sqrt(Math.Pow(dir.x, 2) + Math.Pow(dir.z, 2));
            var theta = (Math.PI / 2) - Math.Atan(dir.y / rSinTheta);
            var x = CANVAS_X_MIN + r * phi;
            var y = Math.Cos(theta) * r + (CANVAS_Y_MAX + CANVAS_Y_MIN);
            
            DrawHeatmap(heatmapData, (int) Math.Round(x), (int) Math.Round(y));
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = Color.Lerp(Color.blue, Color.red, heatmapData[x, y]);
                heatmapTexture.SetPixel(x, y, color);
            }
        }
        
        heatmapTexture.Apply();
        _heatmapSprite = Sprite.Create(heatmapTexture, new Rect(0.0f, 0.0f, width, height), new Vector2(0.5f, 0.5f), 100.0f);
        heatmapImage.sprite = _heatmapSprite;
    }

    private void DrawHeatmap(float[,] heatmapData, int x, int y)
    {
        var width = heatmapData.GetLength(0);
        var height = heatmapData.GetLength(1);

        for (int i = -HEATMAP_RADIUS; i < HEATMAP_RADIUS; i++)
        {
            for (int j = -HEATMAP_RADIUS; j < HEATMAP_RADIUS; j++)
            {
                var dist = Math.Sqrt(Math.Pow(i, 2) + Math.Pow(j, 2));
                if (dist > HEATMAP_RADIUS)
                {
                    continue;
                }

                var wrapX = MathUtils.Mod(x + i, width);
                var wrapY = MathUtils.Mod(y + j, height);
                var heat = (float)(1 - dist / HEATMAP_RADIUS);
                if (heatmapData[wrapX, wrapY] < heat)
                {
                    heatmapData[wrapX, wrapY] = heat;
                }
            }   
        }
    }

    private void TrackGazeData()
    {
        Debug.DrawRay(transform.position, mainCamera.forward * 10, Color.red);
        _gazeLog.Add(mainCamera.forward);
    }

    public void OnLoggingStart()
    {
        _gazeLog = new();
        Debug.Log("Logging start");
        _isLogging = true;
    }

    public void OnLoggingStop()
    {
        Debug.Log("Logging stop");
        _isLogging = false;
        var gazeData = JsonUtility.ToJson(_gazeLog);
        PlayerPrefs.SetString("GazeData", gazeData); // TODO: save multiple data
        PlayerPrefs.Save();
        // _gazeLog = new();
    }
}
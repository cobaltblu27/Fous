using System;
using System.Collections.Generic;
using System.IO;
using Codes.Utils;
using UnityEngine;
using UnityEngine.UI;

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

    private bool _isLogging = false;
    private List<Vector3> _gazeLog = new()
    {
        new Vector3(1,0,0),
        new Vector3(0,0,1),
    };

    [Serializable]
    private class VectorListWrapper
    {
        public List<Vector3> gazeLog;
    }

    void Update()
    {
        if (_isLogging)
        {
            TrackGazeData();
        }
    }

    private void Start()
    {
        var path = Application.persistentDataPath + $"/gaze_log_{DateTime.Now.ToString("yyyyMMddHH_mmss")}.json";
        Debug.Log(path);

        RenderHeatmap();
    }

    private List<Vector3> LoadGazeLog()
    {
        var json = PlayerPrefs.GetString("GazeData", "[]");
        Debug.Log(json);
        var wrapper = JsonUtility.FromJson<VectorListWrapper>(json);
        return wrapper.gazeLog;
    }

    public void RenderHeatmap()
    {
        var gazeLog = LoadGazeLog();
        var sampleSize = gazeLog.Count;
        var width = CANVAS_X_MAX - CANVAS_X_MIN;
        var height = CANVAS_Y_MAX - CANVAS_Y_MIN;
        var r = (CANVAS_X_MAX - CANVAS_X_MIN) / (Math.PI * 2);
        float[,] heatmapData = new float[width, height];
        Texture2D heatmapTexture = new Texture2D(width, height);

        foreach (Vector3 dir in gazeLog)
        {
            var phi = Math.Atan(dir.x / dir.z) + Math.PI;
            var rSinTheta = Math.Sqrt(Math.Pow(dir.x, 2) + Math.Pow(dir.z, 2));
            var theta = (Math.PI / 2) - Math.Atan(dir.y / rSinTheta);
            var x = CANVAS_X_MIN + r * phi;
            var y = Math.Cos(theta) * r + (CANVAS_Y_MAX + CANVAS_Y_MIN);

            DrawHeatmap(heatmapData, (int)Math.Round(x), (int)Math.Round(y));
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var data = heatmapData[x, y];
                Color color = FloatToColor(data);
                heatmapTexture.SetPixel(x, y, color);
            }
        }

        heatmapTexture.Apply();
        _heatmapSprite = Sprite.Create(heatmapTexture, new Rect(0.0f, 0.0f, width, height), new Vector2(0.5f, 0.5f),
            100.0f);
        heatmapImage.sprite = _heatmapSprite;
    }

    private Color FloatToColor(float data)
    {
        if (data < 0.33)
        {
            return Color.Lerp(Color.black, Color.red, data / 0.33f);
        }

        if (data < 0.66)
        {
            return Color.Lerp(Color.red, Color.yellow, (data - 0.33f) / 0.33f);
        }

        return Color.Lerp(Color.yellow, Color.green, (data - 0.66f) / 0.33f);
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
                // heatmapData[wrapX, wrapY] += heat;
                if (heatmapData[wrapX, wrapY] < heat)
                {
                    heatmapData[wrapX, wrapY] = heat;
                }
            }
        }
        //
        // float max = 1;
        // for (int i = 0; i < width; i++)
        // {
        //     for (int j = 0; j < height; j++)
        //     {
        //         if (max < heatmapData[i, j])
        //         {
        //             max = heatmapData[i, j];
        //         }
        //     }
        // }
        // Debug.Log("max:");
        // Debug.Log(max);
        // for (int i = 0; i < width; i++)
        // {
        //     for (int j = 0; j < height; j++)
        //     {
        //         heatmapData[i, j] /= max;
        //     }
        // }
    }

    private void TrackGazeData()
    {
        Debug.DrawRay(transform.position, mainCamera.forward * 10, Color.red);
        _gazeLog.Add(mainCamera.forward);
    }

    public void StartLogging()
    {
        Debug.Log("Logging start");
        _isLogging = true;
    }

    public void PauseLogging()
    {
        _isLogging = false;
    }

    public void StopLogging()
    {
        Debug.Log("Logging stop");
        _isLogging = false;
        
        var wrapper = new VectorListWrapper { gazeLog = _gazeLog };
        string gazeData = JsonUtility.ToJson(wrapper);
        var destination = Application.persistentDataPath + $"/gaze_log_{DateTime.Now.ToString("yyyyMMddHH_mmss")}.json";
        var file = File.Exists(destination) ? File.OpenWrite(destination) : File.Create(destination);

        using (StreamWriter writer = new StreamWriter(file))
        {
            writer.Write(gazeData);
        }

        file.Close();
        Debug.Log(destination);

        PlayerPrefs.SetString("GazeData", gazeData);
        PlayerPrefs.Save();
        
        _gazeLog = new();
    }
}
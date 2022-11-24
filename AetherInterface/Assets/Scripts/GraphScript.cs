using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

public class GraphScript : MonoBehaviour {
    float timer;

    public int nPoints = 12;
    public float lnWidth = 3.0f;
    int nLines;

    float[] values;

    GameObject[] lines;

    float lower = 2;
    float upper = 5;

    float curMax; // Current max value
    float curMin; // Current min value

    float height; // Width of graph in pixels
    float width; // Width of graph in pixels

    float iWidth; // Interval width
    float rangeCo = 1.2f; // Range cooefficient (changes how much graph can be seen)

    float mean;
    float sd;

	// Use this for initialization
	void Start () {
        timer = 0.0f;
        height = 300;
        width = 400;

        

        nLines = nPoints - 1;
        iWidth = width / nLines;
        values = new float[nPoints];
        lines = new GameObject[nLines];

        curMin = -1.0f;
        curMax = -1.0f;

        // Range 2 - 5;
        for (int i = 0; i < nPoints; i++) {
            values[i] = Random.Range(lower, upper);
            Debug.Log(i + ": " + values[i]);
            if (values[i] > curMax || curMax < 0) {
                curMax = values[i];
            }

            if (values[i] < curMin || curMin < 0) {
                curMin = values[i];
            }
        }

        Debug.Log("Min: " + curMin);
        Debug.Log("Max: " + curMax);

        CalcStats();

        Debug.Log("Mean: " + mean);
        Debug.Log("Standard Deviation: " + sd);

        for (int i = 0; i < nLines; i++) {
            lines[i] = new GameObject();
            RectTransform rect = lines[i].AddComponent<RectTransform>();
            Color color;
            ColorUtility.TryParseHtmlString("#0A62EEFF", out color);
            ProceduralImage img = lines[i].AddComponent<ProceduralImage>();
            img.color = color;
            img.FalloffDistance = 0.0001f;
            img.ModifierType = typeof(RoundModifier);

            lines[i].transform.SetParent(transform);
            lines[i].name = "Line" + i;

            // Set default values
            rect.localPosition = Vector3.zero;
            rect.localRotation = Quaternion.identity;
            rect.localScale = new Vector3(1, 1, 1);

            PlaceLine(lines[i].GetComponent<RectTransform>(), new Vector2(i * iWidth, GetInRange(values[i])),
                                                              new Vector2((i + 1) * iWidth, GetInRange(values[i + 1])));
        }


    }

    float GetInRange(float f) {
        float newMin = (curMin * (1 + rangeCo) + curMax * (1 - rangeCo)) / 2.0f;
        float newMax = (curMin * (1 - rangeCo) + curMax * (1 + rangeCo)) / 2.0f;
        return (f - newMin) / (newMax - newMin) * height;
    }

    void PlaceLine(RectTransform rect, Vector2 p0, Vector2 p1) {
        Vector2 newPos = (p0 + p1) / 2.0f;
        rect.localPosition = new Vector3(newPos.x, newPos.y, 0.0f) - new Vector3(width / 2.0f, height / 2.0f, 0);

        float x = p0.x - p1.x;
        float y = p0.y - p1.y;
        float length = Mathf.Sqrt(x * x + y * y) + lnWidth;

        rect.sizeDelta = new Vector2(length, lnWidth);
        rect.localRotation = Quaternion.Euler(0, 0, Mathf.Atan(y/x) * 180.0f / Mathf.PI);
    }

    void CalcStats() {
        mean = 0;
        for (int i = 0; i < nPoints; i++) {
            mean += values[i];
        }
        mean /= nPoints;


        float variance = 0;
        for (int i = 0; i < nPoints; i++) {
            variance += (values[i] - mean) * (values[i] - mean);
        }
        variance /= nPoints;
        sd = Mathf.Sqrt(variance);
    }

	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        if (timer >= 10.0f) {
            timer -= 10.0f;

            // Add new value
        }
	}
}

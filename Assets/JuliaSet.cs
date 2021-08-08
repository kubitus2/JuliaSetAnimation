using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JuliaSet : MonoBehaviour
{

    private double width, height;
    private double rStart, iStart;
    private int maxIterations;

    private ComputeBuffer buffer;
    private RenderTexture texture;

    [SerializeField]
    private double magnitude = 0.7885;

    [SerializeField]
    private ComputeShader shader;

    [SerializeField]
    private RawImage image;

    [SerializeField]
    private double realC, imagC;

    private struct DataStruct
    {
        public double w, h, r, i;
        public double cx, cy;
        public int screenWidth, screenHeight;
    }

    DataStruct[] data;
    void Awake()
    {
        Application.targetFrameRate = 25;
    }

    void Start()
    {
        width = 4.5;
        height = width * Screen.height / Screen.width;
        rStart = -2.0;
        iStart = -1;
        maxIterations = 1000;

        data = new DataStruct[1];

        data[0] = new DataStruct
        {
            w = width,
            h = height,
            r = rStart,
            i = iStart,

            cx = realC,
            cy = imagC,

            screenWidth = Screen.width,
            screenHeight = Screen.height
        };

        buffer = new ComputeBuffer(data.Length, 56);
        texture = new RenderTexture(Screen.width, Screen.height, 0);
        texture.enableRandomWrite = true;
        texture.Create();

        AngleToComplex(Mathf.PI);

        DrawJulia();
    }

    void DrawJulia()
    {
        int kernelHandle = shader.FindKernel("CSMain");

        buffer.SetData(data);
        shader.SetBuffer(kernelHandle, "buffer", buffer);

        shader.SetTexture(kernelHandle, "Result", texture);

        shader.SetInt("maxIterations", maxIterations);
        shader.SetTexture(kernelHandle, "Result", texture);

        shader.Dispatch(kernelHandle, Screen.width / 16, Screen.height / 16, 1);

        RenderTexture.active = texture;
        image.material.mainTexture = texture;
    }

    void AngleToComplex(double angle)
    {
        data[0].cx = magnitude * (double) Mathf.Cos((float) angle);
        data[0].cy = magnitude * (double) Mathf.Sin((float) angle);
    }

    void Update()
    {
        AngleToComplex((Mathf.PI / 30.0f) * Time.time);
        DrawJulia();
    }

    private void OnDestroy()
    {
        buffer.Dispose();
    }


    
}

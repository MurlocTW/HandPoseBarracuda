using System;
using System.Collections;
using System.Collections.Generic;
using MediaPipe.BlazePalm;
using UnityEngine;
using UnityEngine.UI;

public class BlazePalmTest : MonoBehaviour
{
    [SerializeField] WebcamInput _webcam = null;
    [SerializeField] RawImage _previewUI = null;
    [SerializeField] ResourceSet _resources = null;
    [SerializeField] Shader _shader = null;
    
    PalmDetector _detector;
    Material _material;

    ComputeBuffer _boxDrawArgs;
    ComputeBuffer _keyDrawArgs;
    
    private void Start()
    {
        _detector = new PalmDetector(_resources);
        _material = new Material(_shader);

        var cbType = ComputeBufferType.IndirectArguments;
        _boxDrawArgs = new ComputeBuffer(4, sizeof(uint), cbType);
        _keyDrawArgs = new ComputeBuffer(4, sizeof(uint), cbType);
        _boxDrawArgs.SetData(new [] {6, 0, 0, 0});
        _keyDrawArgs.SetData(new [] {24, 0, 0, 0});
    }

    private void OnDestroy()
    {
        _detector.Dispose();
        Destroy(_material);

        _boxDrawArgs.Dispose();
        _keyDrawArgs.Dispose();
    }

    private void LateUpdate()
    {
        _detector.ProcessImage(_webcam.Texture);
        _previewUI.texture = _webcam.Texture;
    }

    private void OnRenderObject()
    {
        // Detection buffer
        _material.SetBuffer("_Detections", _detector.DetectionBuffer);

        // Copy the detection count into the indirect draw args.
        _detector.SetIndirectDrawCount(_boxDrawArgs);
        _detector.SetIndirectDrawCount(_keyDrawArgs);
        
        // Bounding box
        _material.SetPass(0);
        Graphics.DrawProceduralIndirectNow(MeshTopology.Triangles, _boxDrawArgs, 0);

        // Key points
        _material.SetPass(1);
        Graphics.DrawProceduralIndirectNow(MeshTopology.Lines, _keyDrawArgs, 0);
    }
}

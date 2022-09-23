using MediaPipe.BlazePalm;
using MediaPipe.HandLandmark;
using UnityEngine;

namespace MediaPipe.HandPose {

//
// Basic implementation of the hand pipeline class
//

sealed partial class HandPipeline : System.IDisposable
{
    #region Private objects

    const int CropSize = HandLandmarkDetector.ImageSize;

    int InputWidth => _detector.palm.ImageSize;

    ResourceSet _resources;

    (PalmDetector palm, HandLandmarkDetector landmark) _detector;
    
    private ComputeBuffer _inputBuffer;
    private ComputeBuffer _cropBuffer;
    private ComputeBuffer _regionBuffer;
    private ComputeBuffer _filterBuffer;
    
    #endregion

    #region Object allocation/deallocation

    void AllocateObjects(ResourceSet resources)
    {
        _resources = resources;

        _detector = (new PalmDetector(_resources.blazePalm),
                     new HandLandmarkDetector(_resources.handLandmark));

        var inputBufferLength = 3 * InputWidth * InputWidth;
        var cropBufferLength = 3 * CropSize * CropSize;
        var regionStructSize = sizeof(float) * 24;
        var filterBufferLength = HandLandmarkDetector.VertexCount * 2;

        _inputBuffer = new ComputeBuffer(inputBufferLength, sizeof(float));
        _cropBuffer = new ComputeBuffer(cropBufferLength, sizeof(float));
        _regionBuffer = new ComputeBuffer(1, regionStructSize);
        _filterBuffer = new ComputeBuffer(filterBufferLength, sizeof(float) * 4);
    }

    void DeallocateObjects()
    {
        _detector.palm.Dispose();
        _detector.landmark.Dispose();
        
        _inputBuffer.Dispose();
        _cropBuffer.Dispose();
        _regionBuffer.Dispose();
        _filterBuffer.Dispose();
    }

    #endregion
}

} // namespace MediaPipe.HandPose

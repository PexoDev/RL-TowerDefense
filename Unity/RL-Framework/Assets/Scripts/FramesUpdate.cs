public struct FramesUpdate
{
    public int FrameCount { get; private set; }

    public FramesUpdate(int frameCount)
    {
        FrameCount = frameCount;
    }
}
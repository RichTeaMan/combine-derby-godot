using System;
using Godot;

public sealed class WheelAnchor
{

    public Vector3 AttachmentPoint { get; private set; }
    public bool IsTraction { get; private set; }
    public bool IsSteering { get; private set; }

    public Vector3 BaseRotation { get; private set; } = Vector3.Zero;

    public WheelAnchor(Vector3 attachmentPoint, bool isTraction, bool isSteering)
    {

        AttachmentPoint = attachmentPoint;
        IsTraction = isTraction;
        IsSteering = isSteering;

        // guess rotation
        if (AttachmentPoint.X < 0)
        {
            BaseRotation = new Vector3(0.0f, (float)Math.PI, 0.0f);
        }
    }
}

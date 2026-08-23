using Koturn.LilToonCustomGenerator.Editor.Enums;


namespace Koturn.LilToonCustomGenerator.Editor
{
    /// <summary>
    /// v2f struct member definition.
    /// </summary>
    [System.Runtime.InteropServices.Guid("b3765b01-ece0-a244-3989-ca279a4a47dd")]
    public struct VRChatVariableFlagBits
    {
        /// <summary>
        /// Flag value.
        /// </summary>
        public VRChatVariableFlags Value { get; set; }

        /// <summary>
        /// True to declare the uniform variable `UseVRChatCameraMode`.
        /// </summary>
        public bool UseVRChatCameraMode
        {
            readonly get => (Value & VRChatVariableFlags.UseVRChatCameraMode) != 0;
            set => Value = value ? (Value | VRChatVariableFlags.UseVRChatCameraMode) : (Value & ~VRChatVariableFlags.UseVRChatCameraMode);
        }
        /// <summary>
        /// True to declare the uniform variable `UseVRChatCameraMask`.
        /// </summary>
        public bool UseVRChatCameraMask
        {
            readonly get => (Value & VRChatVariableFlags.UseVRChatCameraMask) != 0;
            set => Value = value ? (Value | VRChatVariableFlags.UseVRChatCameraMask) : (Value & ~VRChatVariableFlags.UseVRChatCameraMask);
        }
        /// <summary>
        /// True to declare the uniform variable `UseVRChatMirrorMode`.
        /// </summary>
        public bool UseVRChatMirrorMode
        {
            readonly get => (Value & VRChatVariableFlags.UseVRChatMirrorMode) != 0;
            set => Value = value ? (Value | VRChatVariableFlags.UseVRChatMirrorMode) : (Value & ~VRChatVariableFlags.UseVRChatMirrorMode);
        }
        /// <summary>
        /// True to declare the uniform variable `UseVRChatFaceMirrorMode`.
        /// </summary>
        public bool UseVRChatFaceMirrorMode
        {
            readonly get => (Value & VRChatVariableFlags.UseVRChatFaceMirrorMode) != 0;
            set => Value = value ? (Value | VRChatVariableFlags.UseVRChatFaceMirrorMode) : (Value & ~VRChatVariableFlags.UseVRChatFaceMirrorMode);
        }
        /// <summary>
        /// True to declare the uniform variable `UseVRChatMirrorCameraPos`.
        /// </summary>
        public bool UseVRChatMirrorCameraPos
        {
            readonly get => (Value & VRChatVariableFlags.UseVRChatMirrorCameraPos) != 0;
            set => Value = value ? (Value | VRChatVariableFlags.UseVRChatMirrorCameraPos) : (Value & ~VRChatVariableFlags.UseVRChatMirrorCameraPos);
        }
        /// <summary>
        /// True to declare the uniform variable `UseVRChatScreenCameraPos`.
        /// </summary>
        public bool UseVRChatScreenCameraPos
        {
            readonly get => (Value & VRChatVariableFlags.UseVRChatScreenCameraPos) != 0;
            set => Value = value ? (Value | VRChatVariableFlags.UseVRChatScreenCameraPos) : (Value & ~VRChatVariableFlags.UseVRChatScreenCameraPos);
        }
        /// <summary>
        /// True to declare the uniform variable `UseVRChatScreenCameraRot`.
        /// </summary>
        public bool UseVRChatScreenCameraRot
        {
            readonly get => (Value & VRChatVariableFlags.UseVRChatScreenCameraRot) != 0;
            set => Value = value ? (Value | VRChatVariableFlags.UseVRChatScreenCameraRot) : (Value & ~VRChatVariableFlags.UseVRChatScreenCameraRot);
        }
        /// <summary>
        /// True to declare the uniform variable `UseVRChatPhotoCameraPos`.
        /// </summary>
        public bool UseVRChatPhotoCameraPos
        {
            readonly get => (Value & VRChatVariableFlags.UseVRChatPhotoCameraPos) != 0;
            set => Value = value ? (Value | VRChatVariableFlags.UseVRChatPhotoCameraPos) : (Value & ~VRChatVariableFlags.UseVRChatPhotoCameraPos);
        }
        /// <summary>
        /// True to declare the uniform variable `UseVRChatPhotoCameraRot`.
        /// </summary>
        public bool UseVRChatPhotoCameraRot
        {
            readonly get => (Value & VRChatVariableFlags.UseVRChatPhotoCameraRot) != 0;
            set => Value = value ? (Value | VRChatVariableFlags.UseVRChatPhotoCameraRot) : (Value & ~VRChatVariableFlags.UseVRChatPhotoCameraRot);
        }
        /// <summary>
        /// True to declare the uniform variable `UseVRChatTimeUTCUnixSeconds`.
        /// </summary>
        public bool UseVRChatTimeUTCUnixSeconds
        {
            readonly get => (Value & VRChatVariableFlags.UseVRChatTimeUTCUnixSeconds) != 0;
            set => Value = value ? (Value | VRChatVariableFlags.UseVRChatTimeUTCUnixSeconds) : (Value & ~VRChatVariableFlags.UseVRChatTimeUTCUnixSeconds);
        }
        /// <summary>
        /// True to declare the uniform variable `UseVRChatTimeNetworkMs`.
        /// </summary>
        public bool UseVRChatTimeNetworkMs
        {
            readonly get => (Value & VRChatVariableFlags.UseVRChatTimeNetworkMs) != 0;
            set => Value = value ? (Value | VRChatVariableFlags.UseVRChatTimeNetworkMs) : (Value & ~VRChatVariableFlags.UseVRChatTimeNetworkMs);
        }
        /// <summary>
        /// True to declare the uniform variable `UseVRChatTimeEncoded1`.
        /// </summary>
        public bool UseVRChatTimeEncoded1
        {
            readonly get => (Value & VRChatVariableFlags.UseVRChatTimeEncoded1) != 0;
            set => Value = value ? (Value | VRChatVariableFlags.UseVRChatTimeEncoded1) : (Value & ~VRChatVariableFlags.UseVRChatTimeEncoded1);
        }
        /// <summary>
        /// True to declare the uniform variable `UseVRChatTimeEncoded2`.
        /// </summary>
        public bool UseVRChatTimeEncoded2
        {
            readonly get => (Value & VRChatVariableFlags.UseVRChatTimeEncoded2) != 0;
            set => Value = value ? (Value | VRChatVariableFlags.UseVRChatTimeEncoded2) : (Value & ~VRChatVariableFlags.UseVRChatTimeEncoded2);
        }

        /// <summary>
        /// Set initial value.
        /// </summary>
        /// <param name="val">Initial value.</param>
        public VRChatVariableFlagBits(VRChatVariableFlags val)
        {
            Value = val;
        }
    }
}

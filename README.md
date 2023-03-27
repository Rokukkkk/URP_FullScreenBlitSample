# URP_FullScreenBlitSample
This is a sample for using .shader to perform full-screen blit that uses Volume System to control it.
Since URP14, looks like Unity has replaced CommandBuffer.Blit API by using SRP Blitter API, the legacy Blit API may not work anymore, more info: https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@14.0/manual/customize/blit-overview.html

Check Unity Docs for more info: https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@14.0/manual/renderer-features/how-to-fullscreen-blit.html

__PS: The files don't contain a real Ray Marching function, only a simple color blit that is controlled by Volume Override.__
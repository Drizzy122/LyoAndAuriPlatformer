public class SceneFadingOutEventArgs
{
    public float FadeDuration { get; private set; }

    public SceneFadingOutEventArgs(float fadeDuration)
    {
        FadeDuration = fadeDuration;
    }
}

namespace ForumBlazor.Services;

using ForumBlazor.Components;

public class ForumState
{
    public List<Theme> Themes { get; set; } = new List<Theme>();
    public event Action OnChange;
    
    public void NotifyStateChanged() => OnChange?.Invoke();
}

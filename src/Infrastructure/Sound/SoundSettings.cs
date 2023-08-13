using System.ComponentModel.DataAnnotations;

namespace AudioGuestbook.Infrastructure.Sound;

public class SoundSettings
{
    public const string SectionKey = "Sound";

    [Required]
    [Range(0f, 1.0f)]
    public required float MasterVolume { get; set; }
}
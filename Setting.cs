using System.ComponentModel.DataAnnotations;

namespace Bartoker;

public class Setting
{
    [Key]
    public string Key { get; }
    public string Value { get; set; }
}
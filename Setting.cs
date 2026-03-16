using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Bartoker;

[PrimaryKey("Key")]
public class Setting
{
    [Key]
    public string Key { get; }
    public string Value { get; set; }
}
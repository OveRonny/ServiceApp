﻿namespace ServiceApp.UI.Models;

public class PartsModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public int Quantity { get; set; }
    public string Description { get; set; } = string.Empty;
}

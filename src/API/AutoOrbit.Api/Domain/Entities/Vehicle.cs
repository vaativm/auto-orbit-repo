﻿namespace AutoOrbit.Api.Domain.Entities;

public class Vehicle
{
    public int Id { get; set; }
    public string? VIN { get; set; }
    public int Year { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public string? Trim { get; set; }
    public DateTime DateAdded { get; set; }
}

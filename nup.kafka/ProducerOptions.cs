﻿namespace nup.kafka;

public class ProducerOptions
{
    public int PartitionCount { get; set; } = 1;
    public string Username { get; set; }
    public string Password { get; set; }
}
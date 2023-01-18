﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nup.kafka.DatabaseStuff;

public class KafkaMessage
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid Id { get; set; }
    [StringLength(255)]
    public string PartitionKey { get; set; }
    public int Partition { get; set; }
    public long OffSet { get; set; }
    public bool ProcessedSuccefully { get; set; }
    [StringLength(2000)]
    public string ReasonText { get; set; }
    public DateTime RecievedCreatedAt { get; set; }
    public DateTime? FinishedProcessingAt { get; set; }
}
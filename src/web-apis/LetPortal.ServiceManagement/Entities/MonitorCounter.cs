﻿using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetPortal.ServiceManagement.Entities
{
    [EntityCollection(Name = "monitorcounters")]
    [Table("monitorcounters")]
    public class MonitorCounter : Entity
    {
        public string ServiceId { get; set; }

        public Service Service { get; set; }

        public string ServiceName { get; set; }

        public HttpCounter HttpCounter { get; set; }

        public HardwareCounter HardwareCounter { get; set; }

        public DateTime BeatDate { get; set; }

        // Support Report tool
        public int Hour { get; set; }

        public int Minute { get; set; }
    }

    [Table("hardwarecounters")]
    public class HardwareCounter
    {
        public string Id { get; set; }

        public string MonitorCounterId { get; set; }

        public double CpuUsage { get; set; }

        public long MemoryUsed { get; set; }

        public bool IsCpuBottleneck { get; set; }

        public bool IsMemoryThreshold { get; set; }
    }

    [Table("httpcounters")]
    public class HttpCounter
    {
        public string Id { get; set; }

        public string MonitorCounterId { get; set; }

        public DateTime MeansureDateTime { get; set; }

        public int TotalRequestsPerDay { get; set; }

        public double AvgDuration { get; set; }

        public int SuccessRequests { get; set; }

        public int FailedRequests { get; set; }
    }
}

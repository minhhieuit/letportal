﻿using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Core.Services.Models;
using System;

namespace LetPortal.ServiceManagement.Entities
{
    [EntityCollection(Name = "services")]
    public class Service : Entity
    {
        public string Name { get; set; }

        public int InstanceNo { get; set; }

        public ServiceState ServiceState { get; set; }

        public string RunningVersion { get; set; }

        public string IpAddress { get; set; }

        public ServiceHardwareInfo ServiceHardwareInfo { get; set; }

        public bool LoggerNotifyEnable { get; set; }

        public bool HealthCheckNotifyEnable { get; set; }

        public DateTime LastCheckingDate { get; set; }
    }

    public enum ServiceState
    {
        Start,
        Run,
        Shutdown,
        Lost
    }
}

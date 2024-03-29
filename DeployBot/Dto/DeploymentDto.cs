﻿using System;
using DeployBot.Infrastructure.Database;

namespace DeployBot.Dto
{
    public class DeploymentDto
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public DateTime StatusChangedOn { get; set; }
        public DeploymentStatus Status { get; set; }
    }
}

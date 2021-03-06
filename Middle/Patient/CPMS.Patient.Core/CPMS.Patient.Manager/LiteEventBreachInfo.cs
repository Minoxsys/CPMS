﻿using CPMS.Domain;

namespace CPMS.Patient.Manager
{
    public class LiteEventBreachInfo
    {
        public string EventDescription { get; set; }

        public EventBreachStatus Status { get; set; }

        public int DaysForStatus { get; set; }
    }
}

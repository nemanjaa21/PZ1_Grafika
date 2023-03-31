﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ1_Nemanja_Malinovic.Model
{
    public class SwitchEntity
    {
        private long id;
        private string name;
        private double x;
        private double y;
        private string status;

        public long Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                }
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                }
            }
        }

        public double X
        {
            get { return x; }
            set
            {
                if (x != value)
                {
                    x = value;
                }
            }
        }

        public double Y
        {
            get { return y; }
            set
            {
                if (y != value)
                {
                    y = value;
                }
            }
        }

        public string Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                }
            }
        }
        public SwitchEntity(long id, string name, string status, double x, double y)
        {
            Id = id;
            Name = name;
            Status = status;

            double convertedX;
            double convertedY;
            UTMToDecimalConversion.ToLatLon(x, y, 34, out convertedX, out convertedY);

            X = convertedX;
            Y = convertedY;
        }
        public SwitchEntity()
        {

        }

    }
}

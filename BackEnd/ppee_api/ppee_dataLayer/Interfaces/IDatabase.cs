﻿using ppee_dataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ppee_dataLayer.Interfaces
{
    public interface IDatabase
    {

        Task<bool> WriteToDataBase(List<Load> potorsnja);
        

    }
}

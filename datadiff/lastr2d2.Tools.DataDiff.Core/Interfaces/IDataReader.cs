﻿using System.Data;

namespace LastR2D2.Tools.DataDiff.Core.Interfaces
{
    public interface IDataReader
    {
        DataTable Read(DataReadOptions options);
    }
}

//using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QTrek.WebFrontEnd.classes
{
    public class MigrationStatusClass
    {
        public string SourceTable { get; set; }
        public int CurrentSourceRecorderNumber { get; set; }
        public int TotalSourceRecords { get; set; }
        public string TargetTable { get; set; }
        public int RecordsInserted { get; set; }
        public int RecordsUpdated { get; set; }
        public string MergeStatus { get; set; }

        public MigrationStatusClass()
        {
            Clear();
        }

        public void Clear()
        {
            SourceTable = string.Empty;
            CurrentSourceRecorderNumber = TotalSourceRecords = 0;
            TargetTable = string.Empty;
            RecordsInserted = RecordsUpdated = 0;
            MergeStatus = string.Empty;
        }

        public MigrationStatusClass SetInitialValues(string pSourceTable, string pTargetTable)
        { return SetInitialValues(pSourceTable, pTargetTable, $"Migrate {pSourceTable} -> {pTargetTable}"); }
        public MigrationStatusClass SetInitialValues(string pSourceTable, string pTargetTable, string pMergeStatus)
        {
            return new MigrationStatusClass                
            { 
                SourceTable = pSourceTable,
                TargetTable = pTargetTable,
                MergeStatus = pMergeStatus
            };
        }
    }
}

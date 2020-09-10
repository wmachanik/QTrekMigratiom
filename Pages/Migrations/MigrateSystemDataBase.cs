using iSele.Models.Lookups;
using iSele.Models.System;
using Microsoft.AspNetCore.Components;
using QTrek.Source.Interfaces;
using QTrek.Source.Models;
using QTrek.Target.Interfaces;
using QTrek.WebFrontEnd.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace QTrek.WebFrontEnd.Pages.Migrations
{
    public class MigrateSystemDataBase : ComponentBase
    {

        public MigrationStatusClass MigrationStati = new MigrationStatusClass();
        public bool ShowSpinner = false;

        [Inject]
        public ISourceUnitOfWork _SourceUnitOfWork { get; set; }

        [Inject]
        public ITargetUnitOfWork _TargetUnitOfWork { get; set; }

        public async Task MigrateSysPrefs_Click()
        {
            ShowSpinner = true;
            StateHasChanged();
            await Task.Run(() => MigrateSysPrefs());
            ShowSpinner = false;
            StateHasChanged();
        }
        private void MigrateSysPrefs()
        {
            MigrationStati = MigrationStati.SetInitialValues(nameof(SysDataTbl), nameof(SystemPreferences),"Migrate SysDataTbl -> SystemPreferences");

            ISourceRepository<SysDataTbl> SourceSysDataRepository = _SourceUnitOfWork.SourceRepository<SysDataTbl>();
            IEnumerable<SysDataTbl> SourceSysDatas = SourceSysDataRepository.GetAll();

            ITargetRepository<SystemPreferences> TargetSysDataRepository = _TargetUnitOfWork.TargetRepository<SystemPreferences>();

            foreach (var SourceSysData in SourceSysDatas)
            {
                SystemPreferences TgtSysPrefs = TargetSysDataRepository.GetById(SourceSysData.Id);

                if (TgtSysPrefs == null)
                {

                    TargetSysDataRepository.AddIdDisabled(new SystemPreferences
                    {
                        SystemPreferencesID = SourceSysData.Id,
                        LastReccurringDate = (DateTime)SourceSysData.LastReoccurringDate,
                        DoReccuringOrders = (bool)SourceSysData.DoReoccuringOrders,
                        DateLastPrepDateCalcd = (DateTime)SourceSysData.DateLastPrepDateCalcd,
                        MinReminderDate = (DateTime)SourceSysData.MinReminderDate,
                        GroupItemTypeID = (int)SourceSysData.GroupItemTypeId,
                        DefaultDeliveryPersonID = 3

                    }, "SystemPreferences");
                    MigrationStati.RecordsInserted++;
                }
                else
                {
                    TgtSysPrefs.LastReccurringDate = (DateTime)SourceSysData.LastReoccurringDate;
                    TgtSysPrefs.DoReccuringOrders = (bool)SourceSysData.DoReoccuringOrders;
                    TgtSysPrefs.DateLastPrepDateCalcd = (DateTime)SourceSysData.DateLastPrepDateCalcd;
                    TgtSysPrefs.MinReminderDate = (DateTime)SourceSysData.MinReminderDate;
                    TgtSysPrefs.GroupItemTypeID = (int)SourceSysData.GroupItemTypeId;
                    //                    TgtSysPrefs.DefaultDeliveryPersonID = 3
                    TargetSysDataRepository.Update(TgtSysPrefs);
                    MigrationStati.RecordsUpdated++;
                }
                MigrationStati.TotalSourceRecords++;
            }

        }
        public void MigrateWeekDays_Click()
        {
            MigrationStati = MigrationStati.SetInitialValues(nameof(WeekDaysTbl), "WeekDays", "Migrate WeekDaysTbl -> WeekDays");

            ISourceRepository<WeekDaysTbl> SourceWeekDayRepository = _SourceUnitOfWork.SourceRepository<WeekDaysTbl>();
            IEnumerable<WeekDaysTbl> SourceWeekDays = SourceWeekDayRepository.GetAll();

            ITargetRepository<WeekDay> TargetWeekDayRepository = _TargetUnitOfWork.TargetRepository<WeekDay>();

            foreach (var SourceWeekDay in SourceWeekDays)
            {
                WeekDay TgtWeekDay = TargetWeekDayRepository.FindFirst(wd => wd.WeekDayID == SourceWeekDay.WeekDaysId);

                if (TgtWeekDay == null)
                {

                    TargetWeekDayRepository.AddIdDisabled(new WeekDay
                    {
                        WeekDayID = SourceWeekDay.WeekDaysId,
                        WeekDayName = SourceWeekDay.WeekDayName,
                        IsDispatchDay = (SourceWeekDay.WeekDaysId > 1) && (SourceWeekDay.WeekDaysId < 7)   // we assume 1=sun and 7=sat
                    }, "WeekDays");
                    MigrationStati.RecordsInserted++;
                }
                else
                {
                    TgtWeekDay.WeekDayName = SourceWeekDay.WeekDayName;
                    TgtWeekDay.IsDispatchDay = (SourceWeekDay.WeekDaysId > 1) && (SourceWeekDay.WeekDaysId < 7);   // we assume 1=sun and 7=sat
                    TargetWeekDayRepository.Update(TgtWeekDay);
                    MigrationStati.RecordsUpdated++;
                }
                MigrationStati.TotalSourceRecords++;
            }
        }

        public async Task MigrateTotalCounter_Click()
        {
            ShowSpinner = true;
            StateHasChanged();
            await Task.Run(() => MigrateTotalCounter());
            ShowSpinner = false;
            StateHasChanged();
        }
        public void MigrateTotalCounter()
        {
            MigrationStati.Clear();
            MigrationStati.SourceTable = nameof(TotalCountTrackerTbl);
            MigrationStati.TargetTable = "UsageLog"; // nameof(TotalCountTrackers);
            MigrationStati.MergeStatus = $"Migrate {MigrationStati.SourceTable} -> {MigrationStati.TargetTable}";

            ISourceRepository<TotalCountTrackerTbl> SourceTotalCountTrackerRepository = _SourceUnitOfWork.SourceRepository<TotalCountTrackerTbl>();
            IEnumerable<TotalCountTrackerTbl> SourceTotalCountTrackers = SourceTotalCountTrackerRepository.GetAll();

            ITargetRepository<UsageLog> TargetUsageLogRepository = _TargetUnitOfWork.TargetRepository<UsageLog>();

            foreach (var SourceTotalCountTracker in SourceTotalCountTrackers)
            {
                UsageLog TgtUsageLogTracker = TargetUsageLogRepository.GetById(SourceTotalCountTracker.Id);

                if (TgtUsageLogTracker == null)
                {

                    TargetUsageLogRepository.AddIdDisabled(new UsageLog
                    {
                        UsageLogID = SourceTotalCountTracker.Id,
                        CountDate = (DateTime)SourceTotalCountTracker.CountDate,
                        TotalCount = (int)SourceTotalCountTracker.TotalCount,
                        Comments = SourceTotalCountTracker.Comments
                    }, "UsageLog");
                    MigrationStati.RecordsInserted++;
                }
                else
                {
                    TgtUsageLogTracker.CountDate = (DateTime)SourceTotalCountTracker.CountDate;
                    TgtUsageLogTracker.TotalCount = (int)SourceTotalCountTracker.TotalCount;
                    TgtUsageLogTracker.Comments = SourceTotalCountTracker.Comments;
                    MigrationStati.RecordsUpdated++;
                }
                MigrationStati.TotalSourceRecords++;
            }
        }
        public void MigratePersons_Click()
        {
            MigrationStati.Clear();
            MigrationStati.SourceTable = nameof(PersonsTbl);
            MigrationStati.TargetTable = "Parties"; // nameof(Personss);
            MigrationStati.MergeStatus = $"Migrate {MigrationStati.SourceTable} -> {MigrationStati.TargetTable}";

            ISourceRepository<PersonsTbl> SourcePersonsRepository = _SourceUnitOfWork.SourceRepository<PersonsTbl>();
            IEnumerable<PersonsTbl> SourcePersonss = SourcePersonsRepository.GetAll();

            ITargetRepository<Party> TargetPartyRepository = _TargetUnitOfWork.TargetRepository<Party>();

            foreach (var SourcePersons in SourcePersonss)
            {
                Party TgtPartyTracker = TargetPartyRepository.GetById(SourcePersons.PersonId);

                if (TgtPartyTracker == null)
                {

                    TargetPartyRepository.AddIdDisabled(new Party
                    {
                        PartyID = SourcePersons.PersonId,
                        PartysName = SourcePersons.Person,
                        Abbreviation = SourcePersons.Abreviation,
                        Enabled = (bool)SourcePersons.Enabled,
                        IsForOrderFulfillment = (SourcePersons.Abreviation == "SQ"),
                        NormalDeliveryDoWID = SourcePersons.NormalDeliveryDoW,
                        LoginUserID = SourcePersons.SecurityUsername

                    }, MigrationStati.TargetTable);
                    MigrationStati.RecordsInserted++;
                }
                else
                {

                    TgtPartyTracker.PartysName = SourcePersons.Person;
                    TgtPartyTracker.Abbreviation = SourcePersons.Abreviation;
                    TgtPartyTracker.Enabled = (bool)SourcePersons.Enabled;
                    TgtPartyTracker.IsForOrderFulfillment = (SourcePersons.Abreviation == "SQ");
                    TgtPartyTracker.NormalDeliveryDoWID = SourcePersons.NormalDeliveryDoW;
                    TgtPartyTracker.LoginUserID = SourcePersons.SecurityUsername;
                    MigrationStati.RecordsUpdated++;
                }
                MigrationStati.TotalSourceRecords++;
            }
        }

    }
}

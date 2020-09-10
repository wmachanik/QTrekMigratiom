using iSele.Models.Items;
using iSele.Models.Lookups;
using iSele.Models.System;
using Microsoft.AspNetCore.Components;
using QOnTMSSql.Models;
using QTrek.Source.Interfaces;
using QTrek.Source.Models;
using QTrek.Target.Interfaces;
using QTrek.Tools;
using QTrek.WebFrontEnd.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace QTrek.WebFrontEnd.Pages.Migrations
{
    public class MigrateAddressesBase : ComponentBase
    {

        public MigrationStatusClass MigrationStati = new MigrationStatusClass();
        public bool ShowSpinner = false;

        [Inject]
        public ISourceUnitOfWork _SourceUnitOfWork { get; set; }

        [Inject]
        public ITargetUnitOfWork _TargetUnitOfWork { get; set; }

        public async Task MigrateCity_Click()
        {
            ShowSpinner = true;
            StateHasChanged();
            await Task.Run(() => MigrateCity());
            ShowSpinner = false;
            StateHasChanged();
        }
        void MigrateCity()
        {
            MigrationStati = MigrationStati.SetInitialValues(nameof(ItemGroupTbl), "ItemGroups");

            ISourceRepository<ItemGroupTbl> SourceItemGroupRepository = _SourceUnitOfWork.SourceRepository<ItemGroupTbl>();
            IEnumerable<ItemGroupTbl> SourceItemGroups = SourceItemGroupRepository.GetAll();

            ITargetRepository<ItemGroup> TargetItemGroupRepository = _TargetUnitOfWork.TargetRepository<ItemGroup>();

            foreach (var SourceItemGroup in SourceItemGroups)
            {
                ItemGroup TgtItemGroup = TargetItemGroupRepository.FindFirst(ig => ig.ItemGroupID == SourceItemGroup.ItemGroupId);
                bool _AddRec = (TgtItemGroup == null);
                if (_AddRec)
                {
                    TgtItemGroup = new ItemGroup { ItemGroupID = SourceItemGroup.ItemGroupId };
                }

                TgtItemGroup.GroupItemID = SourceItemGroup.ItemGroupId;
                TgtItemGroup.ItemID = (SourceItemGroup.ItemTypeId == null) ? 0 : (int)SourceItemGroup.ItemTypeId;
                TgtItemGroup.ItemSortPos = (SourceItemGroup.ItemTypeSortPos == null) ? 0 : (int)SourceItemGroup.ItemTypeSortPos;
                TgtItemGroup.IsEnabled = (SourceItemGroup.Enabled == null) ? true : (bool)SourceItemGroup.Enabled;
                TgtItemGroup.Notes = SourceItemGroup.Notes;

                if (_AddRec)
                {
                    TargetItemGroupRepository.AddIdDisabled(TgtItemGroup, "ItemGroups");
                    MigrationStati.RecordsInserted++;
                }
                else
                {
                    TargetItemGroupRepository.Update(TgtItemGroup);
                    MigrationStati.RecordsUpdated++;
                }

                MigrationStati.TotalSourceRecords++;

                //await Task.Run(() => StateHasChanged());
            }
        }
    }
}

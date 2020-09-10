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
    public class MigrateItemsBase : ComponentBase
    {

        public MigrationStatusClass MigrationStati = new MigrationStatusClass();
        public bool ShowSpinner = false;

        [Inject]
        public ISourceUnitOfWork _SourceUnitOfWork { get; set; }

        [Inject]
        public ITargetUnitOfWork _TargetUnitOfWork { get; set; }

        public async Task MigratePackaging_Click()
        {
            ShowSpinner = true;
            StateHasChanged();
            await Task.Run(() => MigratePackaging());
            ShowSpinner = false;
            StateHasChanged();
        }
        private void MigratePackaging()
        {
            MigrationStati = MigrationStati.SetInitialValues(nameof(PackagingTbl), "Packagings", "Migrate PackagingTbl -> Packagings");

            ISourceRepository<PackagingTbl> SourcePacakingRepository = _SourceUnitOfWork.SourceRepository<PackagingTbl>();
            IEnumerable<PackagingTbl> SourcePacakings = SourcePacakingRepository.GetAll();

            ITargetRepository<Packaging> TargetPackagingRepository = _TargetUnitOfWork.TargetRepository<Packaging>();

            foreach (var SourcePacaking in SourcePacakings)
            {
                Packaging TgtPackaging = TargetPackagingRepository.GetById(SourcePacaking.PackagingId);

                if (TgtPackaging == null)
                {

                    TargetPackagingRepository.AddIdDisabled(new Packaging
                    {
                        PackagingID = SourcePacaking.PackagingId,
                        PackagingName = SourcePacaking.Description,
                        AdditionalNotes = SourcePacaking.AdditionalNotes,
                        Symbol = (SourcePacaking.Symbol == null) ? "%" : SourcePacaking.Symbol,
                        Colour = (SourcePacaking.Colour == null) ? 0 : (int)SourcePacaking.Colour,
                        BGColour = (SourcePacaking.Bgcolour == null) ? "#FFF" : SourcePacaking.Bgcolour,
                    }, "Packagings");
                    MigrationStati.RecordsInserted++;
                }
                else
                {
                    TgtPackaging.PackagingName = SourcePacaking.Description;
                    TgtPackaging.AdditionalNotes = SourcePacaking.AdditionalNotes;
                    TgtPackaging.Symbol = (SourcePacaking.Symbol == null) ? "%" : SourcePacaking.Symbol;
                    TgtPackaging.Colour = (SourcePacaking.Colour == null) ? 0 : (int)SourcePacaking.Colour;
                    TgtPackaging.BGColour = (SourcePacaking.Bgcolour == null) ? "#FFF" : SourcePacaking.Bgcolour;
                    TargetPackagingRepository.Update(TgtPackaging);
                    MigrationStati.RecordsUpdated++;
                }
                MigrationStati.TotalSourceRecords++;
            }

        }
        public async void MigratePrep_Click()
        {
            ShowSpinner = true;
            StateHasChanged();
            await Task.Run(() => MigratePrep());
            ShowSpinner = false;
            StateHasChanged();
        }
        public void MigratePrep()
        {
            MigrationStati = MigrationStati.SetInitialValues(nameof(PrepTypesTbl), "Varieties", "Migrate PrepTypesTbl -> Varietys");

            ISourceRepository<PrepTypesTbl> SourcePrepTypeRepository = _SourceUnitOfWork.SourceRepository<PrepTypesTbl>();
            IEnumerable<PrepTypesTbl> SourcePrepTypes = SourcePrepTypeRepository.GetAll();

            ITargetRepository<Variety> TargetVarietyRepository = _TargetUnitOfWork.TargetRepository<Variety>();

            foreach (var SourcePrepType in SourcePrepTypes)
            {
                Variety TgtVariety = TargetVarietyRepository.FindFirst(tv => tv.VarietyID == SourcePrepType.PrepId);

                if (TgtVariety == null)
                {

                    TargetVarietyRepository.AddIdDisabled(new Variety
                    {
                        VarietyID = SourcePrepType.PrepId,
                        VarietyName = SourcePrepType.PrepType,
                        Symbol = SourcePrepType.IdentifyingChar ?? "!",
                    }, "Varieties");
                    MigrationStati.RecordsInserted++;
                }
                else
                {
                    TgtVariety.VarietyName = SourcePrepType.PrepType;
                    TgtVariety.Symbol = SourcePrepType.IdentifyingChar ?? "!";
                    TargetVarietyRepository.Update(TgtVariety);
                    MigrationStati.RecordsUpdated++;
                }
                MigrationStati.TotalSourceRecords++;
            }
        }

        public async Task MigrateItemUnits_Click()
        {
            ShowSpinner = true;
            StateHasChanged();
            await Task.Run(() => MigrateItemUnits());
            ShowSpinner = false;
            StateHasChanged();
        }
        public void MigrateItemUnits()
        {
            MigrationStati = MigrationStati.SetInitialValues(nameof(ItemUnitsTbl), "ItemUnit", "Migrate ItemUnitsTbl -> ItemUnits");

            ISourceRepository<ItemUnitsTbl> SourceItemUnitRepository = _SourceUnitOfWork.SourceRepository<ItemUnitsTbl>();
            IEnumerable<ItemUnitsTbl> SourceItemUnits = SourceItemUnitRepository.GetAll();

            ITargetRepository<ItemUnit> TargetItemUnitRepository = _TargetUnitOfWork.TargetRepository<ItemUnit>();

            foreach (var SourceItemUnit in SourceItemUnits)
            {
                ItemUnit TgtItemUnit = TargetItemUnitRepository.GetById(SourceItemUnit.ItemUnitId);

                if (TgtItemUnit == null)
                {

                    TargetItemUnitRepository.AddIdDisabled(new ItemUnit
                    {
                        ItemUnitID = SourceItemUnit.ItemUnitId,
                        UnitName = SourceItemUnit.UnitDescription,
                        UnitOfMeasure = SourceItemUnit.UnitOfMeasure
                    }, "ItemUnits");
                    MigrationStati.RecordsInserted++;
                }
                else
                {
                    TgtItemUnit.UnitOfMeasure = SourceItemUnit.UnitOfMeasure;
                    TgtItemUnit.UnitName = SourceItemUnit.UnitDescription;
                    TargetItemUnitRepository.Update(TgtItemUnit);
                    MigrationStati.RecordsUpdated++;
                }
                MigrationStati.TotalSourceRecords++;
            }
        }
        public async Task MigrateServiceTypes_Click()
        {
            ShowSpinner = true;
            StateHasChanged();
            await Task.Run(() => MigrateServiceTypes());
            ShowSpinner = false;
            StateHasChanged();
        }
        private void MigrateServiceTypes()
        {
            MigrationStati = MigrationStati.SetInitialValues(nameof(ServiceTypesTbl), "ServiceTypes", "Migrate ServiceTypesTbl -> ServiceTypes");

            ISourceRepository<ServiceTypesTbl> SourceServiceTypeRepository = _SourceUnitOfWork.SourceRepository<ServiceTypesTbl>();
            IEnumerable<ServiceTypesTbl> SourceServiceTypes = SourceServiceTypeRepository.GetAll();

            ITargetRepository<ItemType> TargetItemTypeRepository = _TargetUnitOfWork.TargetRepository<ItemType>();

            foreach (var SourceServiceType in SourceServiceTypes)
            {
                ItemType TgtItemType = TargetItemTypeRepository.FindFirst(it => it.ItemTypeID == SourceServiceType.ServiceTypeId);

                if (TgtItemType == null)
                {

                    TargetItemTypeRepository.AddIdDisabled(new ItemType
                    {
                        ItemTypeID = SourceServiceType.ServiceTypeId,
                        ItemTypeName = SourceServiceType.ServiceType,
                        SortOrder = MigrationStati.TotalSourceRecords
                    }, "ItemTypes");
                    MigrationStati.RecordsInserted++;
                }
                else
                {
                    TgtItemType.ItemTypeName = SourceServiceType.ServiceType;
                    TgtItemType.SortOrder = MigrationStati.TotalSourceRecords;

                    TargetItemTypeRepository.Update(TgtItemType);
                    MigrationStati.RecordsUpdated++;
                }
                MigrationStati.TotalSourceRecords++;
            }
        }
        public async Task MigrateItems_Click()
        {
            ShowSpinner = true;
            StateHasChanged();
            await Task.Run(() => MigrateItems());
            ShowSpinner = false;
            StateHasChanged();
        }
        public void MigrateItems()
        {
            MigrationStati = MigrationStati.SetInitialValues(nameof(ItemTypeTbl), "Items", "Migrate ItemTypeTbl -> Items");

            ISourceRepository<ItemTypeTbl> SourceItemTypesRepository = _SourceUnitOfWork.SourceRepository<ItemTypeTbl>();
            IEnumerable<ItemTypeTbl> SourceItemTypess = SourceItemTypesRepository.GetAll();

            ITargetRepository<Item> TargetItemRepository = _TargetUnitOfWork.TargetRepository<Item>();

            foreach (var SourceItemType in SourceItemTypess)
            {
                Item TgtItem = TargetItemRepository.FindFirst(it => it.ItemID == SourceItemType.ItemTypeId);

                float _BasePrice = (SourceItemType.BasePrice == null) ? 0 : (float)SourceItemType.BasePrice;
                if (TgtItem == null)
                {


                    TargetItemRepository.AddIdDisabled(new Item
                    {

                        ItemID = SourceItemType.ItemTypeId,
                        ItemName = SourceItemType.ItemDesc,
                        SKU = SourceItemType.Sku,
                        IsEnabled = (SourceItemType.ItemEnabled == null) ? true : SourceItemType.ItemEnabled,
                        ItemCharacteritics = SourceItemType.ItemsCharacteritics,
                        ItemDetail = SourceItemType.ItemDetail,
                        ItemTypeID = (SourceItemType.ServiceTypeId == null) ? 0 : (int)SourceItemType.ServiceTypeId,
                        //                        ReplacementItemID = SourceItemType.ReplacementId,
                        ItemAbbreviation = SourceItemType.ItemShortName,
                        SortOrder = (SourceItemType.SortOrder == null) ? (short)0 : (short)SourceItemType.SortOrder,
                        QtyPerUnits = (SourceItemType.UnitsPerQty == null) ? 1 : (decimal)SourceItemType.UnitsPerQty,
                        ItemUnitID = (SourceItemType.ItemUnitId == null) ? 0 : (int)SourceItemType.ItemUnitId,
                        BasePriceIncVAT = (decimal)_BasePrice,
                        BasePriceEXVAT = (decimal)Math.Round(_BasePrice / 1.15, 2),
                        CostPriceIncVAT = (decimal)Math.Round(_BasePrice * .75, 2),
                        CostPriceEXVAT = (decimal)Math.Round((_BasePrice * .75) / 1.15, 2)

                    }, MigrationStati.TargetTable);
                    MigrationStati.RecordsInserted++;
                }
                else
                {
                    TgtItem.ItemName = SourceItemType.ItemDesc;
                    TgtItem.SKU = SourceItemType.Sku;
                    TgtItem.IsEnabled = (SourceItemType.ItemEnabled == null) ? true : SourceItemType.ItemEnabled;
                    TgtItem.ItemCharacteritics = SourceItemType.ItemsCharacteritics;
                    TgtItem.ItemDetail = SourceItemType.ItemDetail;
                    TgtItem.ItemTypeID = (SourceItemType.ServiceTypeId == null) ? 0 : (int)SourceItemType.ServiceTypeId;
                    //                    TgtItem.ReplacementItemID = SourceItemType.ReplacementId;
                    TgtItem.ItemAbbreviation = SourceItemType.ItemShortName;
                    TgtItem.SortOrder = (SourceItemType.SortOrder == null) ? (short)0 : (short)SourceItemType.SortOrder;
                    TgtItem.QtyPerUnits = (SourceItemType.UnitsPerQty == null) ? 1 : (decimal)SourceItemType.UnitsPerQty;
                    TgtItem.ItemUnitID = (SourceItemType.ItemUnitId == null) ? 0 : (int)SourceItemType.ItemUnitId;
                    TgtItem.BasePriceIncVAT = (decimal)_BasePrice;
                    TgtItem.BasePriceEXVAT = (decimal)Math.Round(_BasePrice / 1.15, 2);
                    TgtItem.CostPriceIncVAT = (decimal)Math.Round(_BasePrice * .75, 2);
                    TgtItem.CostPriceEXVAT = (decimal)Math.Round((_BasePrice * .75) / 1.15, 2);

                    TargetItemRepository.Update(TgtItem);
                    MigrationStati.RecordsUpdated++;
                }
                MigrationStati.TotalSourceRecords++;
            }
        }

        public async Task MigrateItemGroups_Click()
        {
            ShowSpinner = true;
            StateHasChanged();
            await Task.Run(() => MigrateItemGroups());
            ShowSpinner = false;
            StateHasChanged();
        }
        async Task MigrateItemGroups()
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

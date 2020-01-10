using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Data.Model.Plan;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.Pages
{
    [AddINotifyPropertyChangedInterface]
    public partial class GarnishesViewModel
    {
        // Dependencies
        private readonly DialogService dialogUtils;
        private readonly GarnishService garnishService;
        private readonly IMapper mapper;

        // State
        public ObservableCollection<GarnishEdit>? Garnishes { get; private set; }
        public bool IsEditing { get; set; }

        // Commands
        public AsyncDelegateCommand LoadedCommand { get; }
        public DelegateCommand AddGarnishCommand { get; }
        public DelegateCommand<GarnishEdit> EditGarnishCommand { get; }
        public DelegateCommand<Guid> DeleteGarnishCommand { get; }

        public GarnishesViewModel(DialogService dialogUtils, GarnishService garnishService, IMapper mapper)
        {
            Debug.Assert(dialogUtils != null);
            Debug.Assert(garnishService != null);
            Debug.Assert(mapper != null);

            this.dialogUtils     = dialogUtils;
            this.garnishService  = garnishService;
            this.mapper          = mapper;
            LoadedCommand        = new AsyncDelegateCommand(OnLoaded, executeOnce: true);
            AddGarnishCommand    = new DelegateCommand(AddGarnish);
            DeleteGarnishCommand = new DelegateCommand<Guid>(DeleteGarnish);
            EditGarnishCommand   = new DelegateCommand<GarnishEdit>(EditGarnish);
        }

        #region Top-level functions
        private Task OnLoaded()
        {
            Debug.WriteLine("GarnishesViewModel.OnLoaded");
            var dbValues = garnishService.GetProjected<GarnishEdit>(mapper);
            Garnishes = new ObservableCollection<GarnishEdit>(dbValues);

            return Task.CompletedTask;
        }

        public async void DeleteGarnish(Guid recipeId)
        {
            await dialogUtils.ShowYesNoDialog("Точно удалить?", "Восстановить будет нельзя",  successCallback: () => OnRecipeDeleted(recipeId))
                             .ConfigureAwait(false);
        }

        public async void AddGarnish()
        {
            await dialogUtils.ShowOkCancelDialog<GarnishEditView, GarnishEditViewModel>("Новый гарнир", successCallback: OnNewGarnishCreated)
                             .ConfigureAwait(false);
        }

        public async void EditGarnish(GarnishEdit garnish)
        {
            var viewModel = new GarnishEditViewModel(mapper.Map<GarnishEdit>(garnish), garnishService, dialogUtils);
            await dialogUtils.ShowOkCancelDialog<GarnishEditView, GarnishEditViewModel>("Редактирование гарнира", viewModel, successCallback: OnGarnishEdited)
                             .ConfigureAwait(false);
        }
        #endregion

        #region Callbacks
        private async void OnRecipeDeleted(Guid recipeId)
        {
            await garnishService.DeleteAsync(recipeId).ConfigureAwait(true);
            Garnishes!.Remove(Garnishes.Single(x => x.ID == recipeId));
        }

        private async void OnGarnishEdited(GarnishEditViewModel viewModel)
        {
            await garnishService.UpdateAsync(mapper.Map<Garnish>(viewModel.Garnish)).ConfigureAwait(false);
            var existingGarnish = Garnishes.Single(x => x.ID == viewModel.Garnish.ID);
            mapper.Map(viewModel.Garnish, existingGarnish);
        }

        private async void OnNewGarnishCreated(GarnishEditViewModel viewModel)
        {
            var id = await garnishService.CreateAsync(mapper.Map<Garnish>(viewModel.Garnish)).ConfigureAwait(false);
            Garnishes!.Add(viewModel.Garnish);
        }
        #endregion
    }
}
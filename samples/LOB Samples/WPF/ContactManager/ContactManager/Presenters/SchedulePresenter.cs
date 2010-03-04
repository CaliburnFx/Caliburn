namespace ContactManager.Presenters
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Caliburn.Core;
    using Caliburn.Core.IoC;
    using Caliburn.ModelFramework;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.RoutedMessaging;
    using Caliburn.PresentationFramework.Screens;
    using Caliburn.WPF.ApplicationFramework;
    using Interfaces;
    using Model;
    using Services.Interfaces;
    using Web;

    [PerRequest(typeof(ISchedulePresenter))]
    public class SchedulePresenter : Screen, ISchedulePresenter
    {
        private readonly IScheduleService _scheduleService;
        private readonly ISettings _settings;
        private DailySchedule _currentSchedule;
        private readonly UndoRedoManager _undoRedoManager;
        private bool _changingSchedule;
        private readonly List<DailySchedule> _alteredSchedules = new List<DailySchedule>();
        private readonly BindableCollection<Contact> _contacts = new BindableCollection<Contact>();

        public SchedulePresenter(IScheduleService scheduleService, ISettings settings)
        {
            _scheduleService = scheduleService;
            _settings = settings;
            _undoRedoManager = new UndoRedoManager();
        }

        public UndoRedoManager UndoRedoManager
        {
            get { return _undoRedoManager; }
        }

        public DailySchedule CurrentSchedule
        {
            get { return _currentSchedule; }
            set
            {
                var previousSchedule = _currentSchedule;

                if(_currentSchedule != null)
                {
                    _undoRedoManager.Unregister(_currentSchedule);
                    _currentSchedule.PropertyChanged -= SchedulePropertyChanged;
                }

                _currentSchedule = value;

                var nextSchedule = _currentSchedule;

                if(_currentSchedule != null)
                {
                    _undoRedoManager.Register(_currentSchedule);

                    if(!_alteredSchedules.Contains(_currentSchedule))
                        _alteredSchedules.Add(_currentSchedule);

                    _currentSchedule.PropertyChanged += SchedulePropertyChanged;
                    _currentSchedule.Validate();
                }

                if(previousSchedule != null &&
                   nextSchedule != null &&
                   !_changingSchedule)
                {
                    _undoRedoManager.Push(
                        () =>{
                            _changingSchedule = true;
                            CurrentSchedule = previousSchedule;
                            _changingSchedule = false;
                        },
                        () =>{
                            _changingSchedule = true;
                            CurrentSchedule = nextSchedule;
                            _changingSchedule = false;
                        });
                }

                NotifyOfPropertyChange(() => CurrentSchedule);
                NotifyOfPropertyChange(() => CanSaveChanges);
            }
        }

        public bool IsDirty
        {
            get
            {
                foreach(var alteredSchedule in _alteredSchedules)
                {
                    if(alteredSchedule.IsDirty)
                        return true;
                }

                return false;
            }
        }

        public bool IsValid
        {
            get
            {
                foreach(var alteredSchedule in _alteredSchedules)
                {
                    if(!alteredSchedule.IsValid)
                        return false;
                }

                return true;
            }
        }

        private void SchedulePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsDirty" || e.PropertyName == "IsValid")
                NotifyOfPropertyChange(() => CanSaveChanges);
        }

        public void GotoPreviousDay()
        {
            CurrentSchedule = _scheduleService.GetScheduleFor(CurrentSchedule.Date.AddDays(-1));
        }

        public void GotoNextDay()
        {
            CurrentSchedule = _scheduleService.GetScheduleFor(CurrentSchedule.Date.AddDays(1));
        }

        public IResult AddAppointment()
        {
            var appointment = new Appointment {Time = _settings.EarliestAppointment};
            _currentSchedule.AddAppointment(appointment);

            if(_contacts.Count > 0)
            {
                appointment.AllContacts = _contacts;
                return null;
            }

            return new WebServiceResult<ContactServiceClient, GetAllContactsCompletedEventArgs>(
                x => x.GetAllContactsAsync(),
                x =>{
                    x.Result.Apply(dto => _contacts.Add(Map.ToContact(dto)));
                    _contacts.Apply(appointment.AllContacts.Add);
                });
        }

        public void RemoveAppointment(Appointment appointmentToRemove)
        {
            _currentSchedule.RemoveAppointment(appointmentToRemove);
        }

        public bool CanSaveChanges
        {
            get { return IsDirty && IsValid; }
        }

        public void SaveChanges()
        {
            _undoRedoManager.Clear();
            _scheduleService.SaveAll();
        }

        public void UndoScheduleChange()
        {
            _undoRedoManager.Undo();
        }

        public void RedoScheduleChange()
        {
            _undoRedoManager.Redo();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            DisplayName = "Daily Schedule";
            CurrentSchedule = _scheduleService.GetScheduleFor(DateTime.Today);
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            _scheduleService.CancelChanges();
        }

        public override bool CanShutdown()
        {
            return !IsDirty;
        }

        public ISubordinate CreateShutdownModel()
        {
            if(IsValid)
            {
                return new Question(
                    this,
                    "The schedule has not been saved.  Do you want to save before closing?"
                    ) {Answer = Answer.Yes};
            }

            return new Question(
                this,
                "The schedule is invalid.  Changes will be lost.  Do you still want to close?",
                Answer.Yes, Answer.No
                );
        }

        public bool CanShutdown(ISubordinate shutdownModel)
        {
            var question = (Question)shutdownModel;

            if(IsValid)
            {
                if(question.Answer == Answer.Cancel)
                    return false;

                if(question.Answer == Answer.Yes)
                    SaveChanges();

                return true;
            }

            if(question.Answer == Answer.Yes)
                return true;

            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;

namespace MVVMcalc.View
{
    public class AdderViewModel : INotifyPropertyChanged
    {

        // Properties

        string currentEntry = "0";
        string historyString = "";

        public string CurrentEntry
        {
            set { SetProperty(ref currentEntry, value); }
            get { return currentEntry; }
        }

        public string HistoryString
        {
            get { return historyString; }
            private set { SetProperty(ref historyString, value); }

        }
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
            {
                return false;
            }
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            }
        }

        public ICommand ClearCommand { private set; get; }
        public ICommand ClearEntryCommand { private set; get; }
        public ICommand BackspaceCommand { private set; get; }
        public ICommand NumericCommand { private set; get; }
        public ICommand DecimalPointCommand { private set; get; }
        public ICommand AddCommand { private set; get; }

        bool blnmIsSumDisplayed = false;
        double intAccumulatedSum = 0;

        /// <summary>
        /// Implementation of the INotifyPropertyChanged Interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
       

      // Constructor 
        public AdderViewModel()
        {
            ClearCommand = new Command(
                execute: () =>
                {
                    HistoryString = "";
                    intAccumulatedSum = 0;
                    CurrentEntry = "0";
                    blnmIsSumDisplayed = false;
                    RefreshCanExecutes();
                });

            ClearEntryCommand = new Command(
                execute: () =>
                {
                    CurrentEntry = "0";
                    blnmIsSumDisplayed = false;
                    RefreshCanExecutes();
                });

            BackspaceCommand = new Command(
                execute: () =>
                {
                    CurrentEntry = CurrentEntry.Substring(0, CurrentEntry.Length - 1);
                    if (CurrentEntry.Length == 0)
                    {
                        CurrentEntry = "0";
                    }

                    RefreshCanExecutes();
                },
                canExecute: () => {
                    return !blnmIsSumDisplayed && (CurrentEntry.Length > 1 || CurrentEntry[0] != '0');
                });

            NumericCommand = new Command<string>(
                execute: (string parameter) =>
                {
                    if (blnmIsSumDisplayed || CurrentEntry == "0")
                    {
                        CurrentEntry = parameter;
                    }
                    else
                    {
                        CurrentEntry += parameter;
                    }

                    blnmIsSumDisplayed = false;
                    RefreshCanExecutes();
                },
                canExecute: (string parameter) =>
                {
                    return blnmIsSumDisplayed || CurrentEntry.Length < 16;
                });

            DecimalPointCommand = new Command(
                execute: () =>
                {
                    if (blnmIsSumDisplayed)
                    {
                        CurrentEntry = "0.";
                    }
                    else
                    {
                        CurrentEntry += ".";
                    }

                    blnmIsSumDisplayed = false;
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    return blnmIsSumDisplayed || !CurrentEntry.Contains(".");
                });

            AddCommand = new Command(
                execute: () =>
                {
                    double value = Double.Parse(CurrentEntry);
                    HistoryString += value.ToString() + " + ";
                    intAccumulatedSum += value;
                    CurrentEntry = intAccumulatedSum.ToString();
                    blnmIsSumDisplayed = true;
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    return !blnmIsSumDisplayed;
                });
        }
       
      // Methods
        void RefreshCanExecutes()
        {
            ((Command)BackspaceCommand).ChangeCanExecute();
            ((Command)NumericCommand).ChangeCanExecute();
            ((Command)DecimalPointCommand).ChangeCanExecute();
            ((Command)AddCommand).ChangeCanExecute();
        }
 
    }
}
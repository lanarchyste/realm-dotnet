using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Realms;
using Xamarin.Forms;

namespace QuickJournal
{
    public class JournalEntriesViewModel
    {
        // TODO: add UI for changing that.
        private const string AuthorName = "Me";

        private Realm _realm;

        public IEnumerable<JournalEntry> Entries { get; private set; }

        public ICommand AddEntryCommand { get; private set; }

        public ICommand ClearAllCommand { get; private set; }

        public ICommand DeleteEntryCommand { get; private set; }

        public INavigation Navigation { get; set; }

        public JournalEntriesViewModel()
        {
            _realm = Realm.GetInstance();

            Entries = _realm.All<JournalEntry>();

            AddEntryCommand = new Command(AddEntry);
            DeleteEntryCommand = new Command<JournalEntry>(DeleteEntry);
            ClearAllCommand = new Command(ClearAll);
        }

        private void AddEntry()
        {
            var transaction = _realm.BeginWrite();
            var entry = _realm.Add(new JournalEntry
            {
                Metadata = new EntryMetadata
                {
                    Date = DateTimeOffset.Now,
                    Author = AuthorName
                }
            });

            var page = new JournalEntryDetailsPage(new JournalEntryDetailsViewModel(entry, transaction));

            Navigation.PushAsync(page);
        }

        private void ClearAll()
        {
            _realm.WriteAsync(async (r) =>
            {
                // pretend awaiting something like a slow download
                // uncomment Task.Delay to cause exception "Cannot access a closed Realm" 
                await Task.Delay(1000);
                r.RemoveAll<JournalEntry>();
            });
        }

        internal void EditEntry(JournalEntry entry)
        {
            var transaction = _realm.BeginWrite();

            var page = new JournalEntryDetailsPage(new JournalEntryDetailsViewModel(entry, transaction));

            Navigation.PushAsync(page);
        }

        private void DeleteEntry(JournalEntry entry)
        {
            _realm.Write(() => _realm.Remove(entry));
        }
    }
}


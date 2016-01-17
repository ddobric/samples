using GuestBook_Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TblStorageSample
{
    class Program
    {
        private static List<GuestBookEntry> m_List = new List<GuestBookEntry>();

        static Program()
        {
            m_List.Add(new GuestBookEntry()
            {
                GuestName = "Damir",
                Message = "Message 01",
                PhotoUrl = "http://upload.wikimedia.org/wikipedia/en/thumb/d/d2/JamesTKirk.jpg/250px-JamesTKirk.jpg",
                MyProperty2 = 1,
            });

            m_List.Add(new GuestBookEntry()
            {
                GuestName = "Fata",
                Message = "Message 02",
                PhotoUrl = "http://t2.gstatic.com/images?q=tbn:ANd9GcQRYjXRjPE1uKtyH8W7R3fuuBBIorQehujSvEkPPNsrutdBInEw",
                MyProperty = 11,
            });
        }


        static void Main(string[] args)
        {

            TblStorageSamples ds = new TblStorageSamples();

            ds.DeleteTables("biztalk");

            deleteEntries();

            addEntries();

            loadAllEntries(ds);

            var res = ds.QueryByMyProperty(538047188);
            var a = res.ToArray();

            var entries = queryTableStorage();

            updateEntry(entries.First());
        }

        private static void loadAllEntries(TblStorageSamples ds)
        {
            var allRecords = ds.SelectAll();
            foreach (var item in allRecords)
            {
                Console.WriteLine(item.Message);
            }
        }

        private static IEnumerable<GuestBookEntry> queryTableStorage()
        {
            TblStorageSamples ds = new TblStorageSamples();

            var res = ds.QueryByName("Angela Merkel");
            foreach (var rec in res)
                Console.WriteLine(rec);

            return res;
        }

        private static void addEntries()
        {
            TblStorageSamples ds = new TblStorageSamples();
            List<GuestBookEntry> books = new List<GuestBookEntry>();
            books.Add(new GuestBookEntry()
            {
                RowKey = "1",
                Message = "Guest book 1",
                GuestName = "Angela Merkel"
            });

            books.Add(new GuestBookEntry()
            {
                RowKey = "2",
                Message = "Guest book 2",
                GuestName = "Barak Obama"
            });


            books.Add(new GuestBookEntry()
            {
                RowKey = "3",
                Message = "Guest book 3",
                GuestName = "Helmut Kohl"
            });


            ds.AddGuestBookEntries(books);

        }


        private static void updateEntry(GuestBookEntry updatingRecord)
        {
            updatingRecord.Message = "Updated";
            TblStorageSamples ds = new TblStorageSamples();
            ds.UpdateGuestBookEntry(updatingRecord);
        }

        private static void deleteEntries()
        {
            TblStorageSamples samples = new TblStorageSamples();

            var allRecords = samples.SelectAll();
            foreach (var item in allRecords)
            {
                Console.WriteLine(item.Message);
            }

            samples.DeleteEntriesSequentially(allRecords.ToArray());
            samples.DeleteEntriesAsBatch(allRecords.ToArray());
        }
    }

}

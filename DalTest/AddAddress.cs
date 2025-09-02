using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalTest
{        //מחלקה ליצירת אובייקטים של כתובות 
     internal class AddAddress
    {
        public string? StringAddress;
        public double Latitude;
        public double Longitude;
    
    
        public static List<AddAddress> GetAddAddresses()
        {
            return new List<AddAddress>()
            {
                new AddAddress() {StringAddress="הרצל 1, תל אביב", Latitude=32.070446, Longitude=34.794667},
                new AddAddress() {StringAddress="שדרות רוטשילד 16, תל אביב", Latitude=32.062819, Longitude=34.774061},
                new AddAddress() {StringAddress="בן גוריון 22, רמת גן", Latitude=32.084027, Longitude=34.812335},
                new AddAddress() {StringAddress="דרך חיפה 101, חיפה", Latitude=32.811834, Longitude=34.983419},
                new AddAddress() {StringAddress="ויצמן 19, כפר סבא", Latitude=32.174547, Longitude=34.905562},
                new AddAddress() {StringAddress="שדרות הנשיא 15, חיפה", Latitude=32.802460, Longitude=34.985614},
                new AddAddress() {StringAddress="הרצל 77, ראשון לציון", Latitude=31.965539, Longitude=34.803267},
                new AddAddress() {StringAddress="דרך בגין 48, תל אביב", Latitude=32.069509, Longitude=34.783370},
                new AddAddress() {StringAddress="שדרות ירושלים 45, יפו", Latitude=32.051958, Longitude=34.758285},
                new AddAddress() {StringAddress="הרצל 60, חדרה", Latitude=32.434046, Longitude=34.918386},
                new AddAddress() {StringAddress="אלנבי 99, תל אביב", Latitude=34.769863, Longitude=32.068588},
                new AddAddress() {StringAddress="בלפור 14, בת ים", Latitude=32.019664, Longitude=34.745525},
                new AddAddress() {StringAddress="ז'בוטינסקי 33, פתח תקווה", Latitude=32.090468, Longitude=34.878975},
                new AddAddress() {StringAddress="הגליל 12, צפת", Latitude=32.965678, Longitude=35.494012},
                new AddAddress() {StringAddress="השומר 10, תל אביב", Latitude=32.068281, Longitude=34.770236},
                new AddAddress() {StringAddress="יגאל אלון 98, תל אביב", Latitude=32.069810, Longitude=34.794895},
                new AddAddress() {StringAddress="שדרות רבין 18, מודיעין", Latitude=31.906328, Longitude=35.004601},
                new AddAddress() {StringAddress="המושבה 7, כפר תבור", Latitude=32.687209, Longitude=35.420792},
                new AddAddress() {StringAddress="עמק רפאים 34, ירושלים", Latitude=31.764492, Longitude=35.220554},
                new AddAddress() {StringAddress="הרצל 9, נהריה", Latitude=33.004863, Longitude=35.092421},
                new AddAddress() {StringAddress="נחלת יצחק 18, תל אביב", Latitude=32.078912, Longitude=34.798176},
                new AddAddress() {StringAddress="סוקולוב 50, הרצליה", Latitude=32.164097, Longitude=34.843212},
                new AddAddress() {StringAddress="ביאליק 25, חולון", Latitude=32.016736, Longitude=34.771445},
                new AddAddress() {StringAddress="דרך בגין 132, תל אביב", Latitude=32.078932, Longitude=34.786432},
                new AddAddress() {StringAddress="העצמאות 35, עכו", Latitude=32.924532, Longitude=35.072145},
                new AddAddress() {StringAddress="זבוטינסקי 7, רמת גן", Latitude=32.083612, Longitude=34.815412},
                new AddAddress() {StringAddress="שדרות מנחם בגין 85, תל אביב", Latitude=32.074522, Longitude=34.789123},
                new AddAddress() {StringAddress="קפלן 15, פתח תקווה", Latitude=32.084932, Longitude=34.886141},
                new AddAddress() {StringAddress="אבן גבירול 22, תל אביב", Latitude=32.077366, Longitude=34.781570},
                new AddAddress() {StringAddress="חיים ויצמן 15, נתניה", Latitude=32.332744 , Longitude=34.853721},
                new AddAddress() {StringAddress="ירושלים 12, רמלה", Latitude=31.928987 , Longitude=34.873123},
                new AddAddress() {StringAddress="שדרות הנשיא 120, חיפה", Latitude=32.802379 , Longitude=34.985995},
                new AddAddress() {StringAddress="בגין 4, עפולה", Latitude=32.609877 , Longitude=35.288423},
                new AddAddress() {StringAddress="יפו 8, חיפה", Latitude=32.816562, Longitude= 34.995884},
                new AddAddress() {StringAddress="שפירא 2, קריית שמונה", Latitude=33.208588 , Longitude=35.570471},
                new AddAddress() {StringAddress="ארלוזורוב 21, נתניה", Latitude=32.329859 , Longitude=34.855738},
                new AddAddress() {StringAddress="דיזנגוף 65, תל אביב", Latitude=32.080126 , Longitude=34.774455},
                new AddAddress() {StringAddress="שדרות הרצל 33, חיפה", Latitude=32.815532 , Longitude=34.995479},
                new AddAddress() {StringAddress="ביאליק 10, רעננה", Latitude=32.184289, Longitude=34.873586},
                new AddAddress() {StringAddress="קיבוץ גלויות 23, תל אביב", Latitude=32.045431, Longitude=34.764147},
                new AddAddress() {StringAddress="רוטשילד 18, ראשון לציון", Latitude=31.960252, Longitude=34.804289},
                new AddAddress() {StringAddress="ראול ולנברג 24, תל אביב", Latitude=32.111427, Longitude=34.840487},
                new AddAddress() {StringAddress="התקווה 7, אשקלון", Latitude=31.670788, Longitude=34.571228},
                new AddAddress() {StringAddress="הר הצופים 3, ירושלים", Latitude=31.793755, Longitude=35.241553},
                new AddAddress() {StringAddress="בר אילן 10, חולון", Latitude=32.013330, Longitude=34.776073},
                new AddAddress() {StringAddress="שלמה המלך 19, אשדוד", Latitude=31.803685, Longitude=34.641451},
                new AddAddress() {StringAddress="בן יהודה 34, תל אביב", Latitude=32.081943, Longitude=34.768878},
                new AddAddress() {StringAddress="בר כוכבא 5, רמת גן", Latitude=32.083171, Longitude=34.814524},
                new AddAddress() {StringAddress="הרצל 8, נתניה", Latitude=32.3320151, Longitude=34.855212}
            };
        }
    }
}

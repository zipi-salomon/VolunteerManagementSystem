using BO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PL
{
    public class ConvertUpdateToTrue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == "Update";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    class ConverterEnumCallTypeToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Brushes.White;
            BO.CallType CallType = (BO.CallType)value;

            switch (CallType)
            {
                case BO.CallType.CookingFood:
                    return Brushes.Pink;
                case BO.CallType.BabysitterServices:
                    return Brushes.Orange;
                case BO.CallType.Transportation:
                    return Brushes.Yellow;
                case BO.CallType.Shopping:
                    return Brushes.YellowGreen;
                case BO.CallType.HouseCleaning:
                    return Brushes.Aqua;
                case BO.CallType.Ironing:
                    return Brushes.White;
                default:
                    return Brushes.White;

            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ConverterEnumStatusCallToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Brushes.White;
            BO.StatusCall statusCall = (BO.StatusCall)value;

            switch (statusCall)
            {
                case BO.StatusCall.Closed:
                    return Brushes.Pink;
                case BO.StatusCall.Open:
                    return Brushes.Yellow;
                case BO.StatusCall.InTreatment:
                    return Brushes.GreenYellow;
                case BO.StatusCall.InTreatmentAtRisk:
                    return Brushes.Gray;
                case BO.StatusCall.Expired:
                    return Brushes.LightBlue;
                case BO.StatusCall.OpenAtRisk:
                    return Brushes.LightCoral;
                default:
                    return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ConvertUpdateToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == "Update"
                ? Visibility.Visible
                :
                Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class ConvertManagerToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object ConverterParameter, CultureInfo culture)
        {

            if (value is BO.Role role)
            {
                bool isManager = role == BO.Role.Manager;
                bool invert = ConverterParameter?.ToString() == "Invert";

                return (isManager ^ invert) ? Visibility.Visible : Visibility.Hidden;
            }

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class ConvertManagerToIsEnabled : IValueConverter
    {
        public object Convert(object value, Type targetType, object ConverterParameter, CultureInfo culture)
        {

            if (value is BO.Role role && role == BO.Role.Manager)
                return true;
            return false;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    public class ConvertCallInProgressToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return value is null ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class ConvertCallInProgressAndActiveToEnable : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            return values[0] is bool b && b && values[1] == null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)

            => throw new NotImplementedException();
    }

    public class MultiValueToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            BO.Volunteer? connect = (BO.Volunteer)values[0] ?? null;
            BO.Volunteer? updated = (BO.Volunteer)values[1] ?? null;
            return connect?.Role == BO.Role.Volunteer ? false : true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    class ConverterOpenOrOpenAtRiskToEnable : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is BO.StatusCall.OpenAtRisk || value is BO.StatusCall.Open;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
    class ConverterInTreatmentOrInTreatmentAtRiskToEnable : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is BO.StatusCall.OpenAtRisk || value is BO.StatusCall.Open|| value is BO.StatusCall.InTreatment||
                value is BO.StatusCall.InTreatmentAtRisk ;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
    public class ConvertCallInProgressToEnable : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return value is null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            !(value is bool b) ? DependencyProperty.UnsetValue : !b;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    public class SimulatorButtonContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (value is bool b && b) ? "עצור סימולטור" : "הפעל סימולטור";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}





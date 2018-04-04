using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Common.Extensions
{
    public static class TimeSpanExtensions
    {

        public static string TimeElapsed(this TimeSpan elapsed)
        {
            if (elapsed.TotalDays >= 365)
            {
                var count = elapsed.TotalDays / 365;
                if (count >= 2)
                {
                    return $"{Math.Truncate(count)} anos";
                }
                else
                {
                    return $"{Math.Truncate(count)} ano";
                }
            }
            else if (elapsed.TotalDays >= 30)
            {
                var count = elapsed.TotalDays / 30;
                if (count >= 2)
                {
                    return $"{Math.Truncate(count)} meses";
                }
                else
                {
                    return $"{Math.Truncate(count)} mês";
                }
            }
            else if (elapsed.TotalHours >= 24)
            {
                var count = elapsed.TotalHours / 24;
                if (count >= 2)
                {
                    return $"{Math.Truncate(count)} dias";
                }
                else
                {
                    return $"{Math.Truncate(count)} dia";
                }
            }
            else if (elapsed.TotalMinutes >= 60)
            {
                var count = elapsed.TotalMinutes / 60;
                if (count >= 2)
                {
                    return $"{Math.Truncate(count)} horas";
                }
                else
                {
                    return $"{Math.Truncate(count)} hora";
                }
            }
            else if (elapsed.TotalSeconds >= 60)
            {
                var count = elapsed.TotalSeconds / 60;
                if (count >= 2)
                {
                    return $"{Math.Truncate(count)} minutos";
                }
                else
                {
                    return $"{Math.Truncate(count)} minuto";
                }
            }
            else
                return "agora mesmo";
        }


    }
}

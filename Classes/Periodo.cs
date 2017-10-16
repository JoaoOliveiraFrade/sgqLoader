using System;

namespace sgq
{
    public class Period
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Period(DateTime? start = null, DateTime? end = null)
        {
            if (start != null)
                this.Start = (DateTime)start;
            else
                this.Start = DateTime.Now;

            if (end != null)
                this.End = (DateTime)end;
            else
                this.End = DateTime.Now;
        }
    }
}

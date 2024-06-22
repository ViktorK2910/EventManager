using System;

namespace EventManagerKubSU.DAL
{
    internal class Event
    {
        public string name_event { get; internal set; }
        public string date { get; internal set; }
        public string venue { get; internal set; }
        public string responsible { get; internal set; }
        public string required_amount { get; internal set; }
        public string comment { get; internal set; }
        public int completed { get; internal set; }
    }
}
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace VPSBot
{
    public enum productType { VPS =1, RDP };
    public enum productOS { Windows =1, Linux };
    public enum WindowOption { OSUses = 1, Hosting};
    public enum LinuxOptions { Mailing = 1, WebHosting, Scripting}

    public enum Processor { OneCore = 1, TwoCore, QuadCore, OctaCore}
    public enum productRAM { OneGB, TwoGB, ThreeGB, FourGB };

    [Serializable]
    public class ProductOrder
    {

        public productType? type;
        [Prompt("Which OS you want? {||}")]
        [Template(TemplateUsage.NotUnderstood,"What does \"{0}\" mean?")]
        [Describe("Type of OS")]
        public productOS os;
        [Describe("option how you want to use")]
        public WindowOption windowsUses;
        [Describe("option How you want to use")]
        public LinuxOptions linuxUses;
        public Processor? processor;
        public productRAM ram;

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("(Type : {0},\n Operating System : {1}, ", type,os);
            switch (os)
            {
                case productOS.Windows:
                    builder.AppendFormat("\n Uses : {0},", windowsUses);
                    break;
                case productOS.Linux:
                    builder.AppendFormat("\n Uses : {0},", linuxUses);
                    break;
            }
            builder.AppendFormat("\n Processor : {0},\n RAM : {1}", processor, ram);
            return builder.ToString();
        }
        //public static IForm<ProductOrder> BuildForm()
        //{
        //    return new FormBuilder<ProductOrder>()
        //        .Message("Welcome to the VPSBot")
        //        .Build();
        //}

    }
}
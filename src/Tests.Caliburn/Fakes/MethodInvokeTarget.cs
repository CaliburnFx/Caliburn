using System;
using System.Windows;

namespace Tests.Caliburn.Fakes
{
    public class MethodInvokeTarget
    {
        public static int ReturnValue = 7;

        public bool InstanceProcedureWasCalled;
        public bool InstanceFunctionWasCalled;
        public static bool StaticFunctionWasCalled;
        public static bool StaticProcedureWasCalled;

        public event EventHandler<RoutedEventArgs> AnEvent;

        public void FireAnEvent()
        {
            if (AnEvent != null)
                AnEvent(this, new RoutedEventArgs());
        }

        public void AnInstanceProcedure()
        {
            InstanceProcedureWasCalled = true;
        }

        public int AnInstanceFunction()
        {
            InstanceFunctionWasCalled = true;
            return ReturnValue;
        }

        public static void AStaticProcedure()
        {
            StaticProcedureWasCalled = true;
        }

        public static int AStaticFunction()
        {
            StaticFunctionWasCalled = true;
            return ReturnValue;
        }

        public static void Reset()
        {
            StaticProcedureWasCalled = false;
            StaticFunctionWasCalled = false;
        }
    }
}
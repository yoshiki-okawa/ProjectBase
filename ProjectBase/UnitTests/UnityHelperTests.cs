using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PB.Frameworks.Common.Unity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PB.UnitTests
{
	public interface IA
	{
		void SaySomething();
	}
	public interface IB
	{
		void SaySomething();
	}
	public class A : IA
	{
		private IUnityContainer container;
		public A(IUnityContainer container)
		{
			this.container = container;
		}
		public void SaySomething()
		{
			Console.WriteLine("Thread " + Thread.CurrentThread.ManagedThreadId + ": A");
		}
	}
	public class B : IB
	{
		private IUnityContainer container;
		private IA a;
		public B(IUnityContainer container)
		{
			this.container = container;
			this.a = container.Resolve<IA>();
		}
		public void SaySomething()
		{
			a.SaySomething();
			Console.WriteLine("Thread " + Thread.CurrentThread.ManagedThreadId + ": B");
		}
	}

	[TestClass]
	public class UnityHelperTests
	{
		[TestMethod]
		public void TestBasicDIUsingUnity()
		{
			var container = new UnityContainer();
			container.RegisterType<IA, A>();
			container.RegisterType<IB, B>();
			IB b = container.Resolve<IB>();
			b.SaySomething();
		}

		[TestMethod]
		public void TestThreadDIUsingUnity()
		{
			var container = UnityHelper.Current;
			container.RegisterType<IA, A>();
			container.RegisterType<IB, B>();
			IB b = container.Resolve<IB>();
			b.SaySomething(); // should say main thread.
			Parallel.Invoke(() => 
			{
				Assert.AreNotEqual(UnityHelper.Current, container);
				Assert.IsFalse(UnityHelper.Current.IsRegistered<IA>(), "IA should not be registered at this point.");
				Assert.IsFalse(UnityHelper.Current.IsRegistered<IB>(), "IB should not be registered at this point.");
				UnityHelper.Current = container;
				Assert.AreEqual(UnityHelper.Current, container);
				Assert.IsTrue(UnityHelper.Current.IsRegistered<IA>(), "IA should be registered now.");
				Assert.IsTrue(UnityHelper.Current.IsRegistered<IB>(), "IB should be registered now.");
				IB b2 = UnityHelper.Current.Resolve<IB>();
				b2.SaySomething();
			});
		}
	}
}

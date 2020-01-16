using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Cooking.WPF.Commands;
using System.Threading.Tasks;
using Moq;
using System.Threading;

namespace Cooking.Tests
{
    [TestClass]
    public class AsyncDelegateCommandTests
    {
        // Positive tests

        [TestMethod]
        public void Execute_ExecutesFunction()
        {
            var funcMock = new Mock<Func<Task>>();

            var asyncDelegate = new AsyncDelegateCommand(funcMock.Object);
            asyncDelegate.Execute();

            // Waiting for async execution to end
            Thread.Sleep(100);

            Assert.AreEqual(1, funcMock.Invocations.Count);
        }

        [TestMethod]
        public void Execute_CanExecuteFunction_NoOptions_ReturnsTrueAsCanExecute()
        {
            var funcMock = new Mock<Func<Task>>();
            var asyncDelegate = new AsyncDelegateCommand(funcMock.Object, canExecute: () => true);
            
            Assert.IsTrue(asyncDelegate.CanExecute());
        }

        [TestMethod]
        public void Execute_CanExecuteFunction_NoOptions_ReturnsFalseAsCanExecute()
        {
            var funcMock = new Mock<Func<Task>>();
            var asyncDelegate = new AsyncDelegateCommand(funcMock.Object, canExecute: () => false);

            Assert.IsFalse(asyncDelegate.CanExecute());
        }

        [TestMethod]
        public void Execute_ExecuteOnce_ReturnsTrueBeforeExecution()
        {
            var funcMock = new Mock<Func<Task>>();
            var asyncDelegate = new AsyncDelegateCommand(funcMock.Object, executeOnce: true);

            Assert.IsTrue(asyncDelegate.CanExecute());
        }

        [TestMethod]
        public void Execute_ExecuteOnce_ReturnsFalseAfterExecution()
        {
            var funcMock = new Mock<Func<Task>>();
            var asyncDelegate = new AsyncDelegateCommand(funcMock.Object, executeOnce: true);
            asyncDelegate.Execute();

            Assert.IsFalse(asyncDelegate.CanExecute());
        }

        // Negative tests
        // Async command does not return exceptions to the caller (UI) because of sync nature of ICommand
        [TestMethod]
        public void Execute_NullInput_ExecutesFunction_ErrorSwallen()
        {
            var asyncDelegate = new AsyncDelegateCommand(null);
            asyncDelegate.Execute();
        }

        [TestMethod]
        public void Execute_FunctionThrow_ExecutesFunction_ErrorSwallen()
        {
            var asyncDelegate = new AsyncDelegateCommand(() => throw new Exception());
            asyncDelegate.Execute();
        }

        // CanExecute is not async - so error is not swallen
        [TestMethod]
        public void Execute_CanExecuteThrow_ErrorNotSwallen()
        {
            var funcMock = new Mock<Func<Task>>();
            var asyncDelegate = new AsyncDelegateCommand(funcMock.Object, canExecute: () => throw new Exception());

            Assert.ThrowsException<Exception>(() => asyncDelegate.CanExecute());
        }
    }
}

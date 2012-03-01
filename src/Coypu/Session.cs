﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Coypu.Actions;
using Coypu.Finders;
using Coypu.Predicates;
using Coypu.Queries;
using Coypu.Robustness;
using Coypu.WebRequests;

namespace Coypu
{
    /// <summary>
    /// A browser session
    /// </summary>
    public class Session : Scope<Session>, IDisposable
    {
        private readonly Driver driver;
        private readonly RestrictedResourceDownloader restrictedResourceDownloader;
        private readonly UrlBuilder urlBuilder;

        internal bool WasDisposed { get; private set; }
        private readonly DriverScope driverScope;

        internal Session(Driver driver, RobustWrapper robustWrapper, Waiter waiter, RestrictedResourceDownloader restrictedResourceDownloader, UrlBuilder urlBuilder)
        {
            driverScope = new DriverScope(new DocumentElementFinder(driver), driver, robustWrapper, waiter, urlBuilder);
            this.driver = driver;
            this.restrictedResourceDownloader = restrictedResourceDownloader;
            this.urlBuilder = urlBuilder;
        }

        internal DriverScope DriverScope
        {
            get { return driverScope; }
        }

        internal Driver Driver
        {
            get { return driver; }
        }

        /// <summary>
        /// The native driver for the Coypu.Driver / browser combination you supplied. E.g. for SeleniumWebDriver + Firefox it will currently be a OpenQA.Selenium.Firefox.FirefoxDriver.
        /// </summary>
        public object Native
        {
            get { return driver.Native; }
        }

        /// <summary>
        /// The current location of the browser
        /// </summary>
        public Uri Location
        {
            get { return DriverScope.Location; }
        }

        /// <summary>
        /// Disposes the current session, closing any open browser.
        /// </summary>
        public void Dispose()
        {
            if (WasDisposed)
                return;

            driver.Dispose();
            WasDisposed = true;
        }

        /// <summary>
        /// Check that a dialog with the specified text appears within the <see cref="Configuration.Timeout"/>
        /// </summary>
        /// <param name="withText">Dialog text</param>
        /// <returns>Whether an element appears</returns>
        public bool HasDialog(string withText)
        {
            return Query(new HasDialogQuery(driver, withText, driverScope.Timeout));
        }

        /// <summary>
        /// Check that a dialog with the specified is not present. Returns as soon as the dialog is not present, or when the <see cref="Configuration.Timeout"/> is reached.
        /// </summary>
        /// <param name="withText">Dialog text</param>
        /// <returns>Whether an element does not appears</returns>
        public bool HasNoDialog(string withText)
        {
            return Query(new HasNoDialogQuery(driver, withText, timeout: driverScope.Timeout));
        }

        /// <summary>
        /// Accept the first modal dialog to appear within the <see cref="Configuration.Timeout"/>
        /// </summary>
        /// <exception cref="T:Coypu.MissingHtmlException">Thrown if the dialog cannot be found</exception>
        public void AcceptModalDialog()
        {
            driverScope.RetryUntilTimeout(new AcceptModalDialog(driver, DriverScope.Timeout));
        }

        /// <summary>
        /// Cancel the first modal dialog to appear within the <see cref="Configuration.Timeout"/>
        /// </summary>
        /// <exception cref="T:Coypu.MissingHtmlException">Thrown if the dialog cannot be found</exception>
        public void CancelModalDialog()
        {
            driverScope.RetryUntilTimeout(new CancelModalDialog(driver,DriverScope.Timeout));
        }

        /// <summary>
        /// Saves a resource from the web to a local file using the cookies from the current browser session.
        /// Allows you to sign in through the browser and then directly download a resource restricted to signed-in users.
        /// </summary>
        /// <param name="resource"> The location of the resource to download</param>
        /// <param name="saveAs">Path to save the file to</param>
        public void SaveWebResource(string resource, string saveAs)
        {
            restrictedResourceDownloader.SetCookies(driver.GetBrowserCookies());
            restrictedResourceDownloader.DownloadFile(urlBuilder.GetFullyQualifiedUrl(resource), saveAs);
        }

        public Session ClickButton(string locator)
        {
            driverScope.ClickButton(locator);
            return this;
        }

        public Session ClickLink(string locator)
        {
            driverScope.ClickLink(locator);
            return this;
        }

        public Session ClickButton(string locator, Func<bool> until, TimeSpan waitBetweenRetries)
        {
            driverScope.ClickButton(locator, until, waitBetweenRetries);
            return this;
        }

        public Session ClickButton(string locator, Predicate until, TimeSpan waitBetweenRetries)
        {
            driverScope.ClickButton(locator, until, waitBetweenRetries);
            return this;
        }

        public Session ClickLink(string locator, Func<bool> until, TimeSpan waitBetweenRetries)
        {
            driverScope.ClickLink(locator, until, waitBetweenRetries);
            return this;
        }

        public Session ClickLink(string locator, Predicate until, TimeSpan waitBetweenRetries)
        {
            driverScope.ClickLink(locator, until, waitBetweenRetries);
            return this;
        }

        /// <summary>
        /// Visit a url in the browser
        /// </summary>
        /// <param name="virtualPath">Virtual paths will use the Configuration.AppHost,Port,SSL settings. Otherwise supply a fully qualified URL.</param>
        public Session Visit(string virtualPath)
        {
            driver.Visit(urlBuilder.GetFullyQualifiedUrl(virtualPath));
            return this;
        }

        public ElementScope FindButton(string locator)
        {
            return driverScope.FindButton(locator);
        }

        public ElementScope FindLink(string locator)
        {
            return driverScope.FindLink(locator);
        }

        public ElementScope FindField(string locator)
        {
            return driverScope.FindField(locator);
        }

        public FillInWith FillIn(string locator)
        {
            return driverScope.FillIn(locator);
        }

        public FillInWith FillIn(Element element)
        {
            return driverScope.FillIn(element);
        }

        public SelectFrom Select(string option)
        {
            return driverScope.Select(option);
        }

        public bool HasContent(string text)
        {
            return driverScope.HasContent(text);
        }

        public bool HasContentMatch(Regex pattern)
        {
            return driverScope.HasContentMatch(pattern);
        }

        public bool HasNoContent(string text)
        {
            return driverScope.HasNoContent(text);
        }

        public bool HasNoContentMatch(Regex pattern)
        {
            return driverScope.HasNoContentMatch(pattern);
        }

        public bool HasCss(string cssSelector)
        {
            return driverScope.HasCss(cssSelector);
        }

        public bool HasNoCss(string cssSelector)
        {
            return driverScope.HasNoCss(cssSelector);
        }

        public bool HasXPath(string xpath)
        {
            return driverScope.HasXPath(xpath);
        }

        public bool HasNoXPath(string xpath)
        {
            return driverScope.HasNoXPath(xpath);
        }

        public ElementScope FindCss(string cssSelector)
        {
            return driverScope.FindCss(cssSelector);
        }

        public ElementScope FindXPath(string xpath)
        {
            return driverScope.FindXPath(xpath);
        }

        public IEnumerable<Element> FindAllCss(string cssSelector)
        {
            return driverScope.FindAllCss(cssSelector);
        }

        public IEnumerable<Element> FindAllXPath(string xpath)
        {
            return driverScope.FindAllXPath(xpath);
        }

        public ElementScope FindSection(string locator)
        {
            return driverScope.FindSection(locator);
        }

        public IFrameElementScope FindIFrame(string locator)
        {
            return driverScope.FindIFrame(locator);
        }

        public ElementScope FindFieldset(string locator)
        {
            return driverScope.FindFieldset(locator);
        }

        public ElementScope FindId(string id)
        {
            return driverScope.FindId(id);
        }

        public Session Check(string locator)
        {
            driverScope.Check(locator);
            return this;
        }

        public Session Uncheck(string locator)
        {
            driverScope.Uncheck(locator);
            return this;
        }

        public Session Choose(string locator)
        {
            driverScope.Choose(locator);
            return this;
        }

        public string ExecuteScript(string javascript)
        {
            return driverScope.ExecuteScript(javascript);
        }

        public Session Hover(Element element)
        {
            driverScope.Hover(element);
            return this;
        }

        public bool Has(ElementScope findElement)
        {
            return driverScope.Has(findElement);
        }

        public bool HasNo(ElementScope findElement)
        {
            return driverScope.HasNo(findElement);
        }

        public void RetryUntilTimeout(Action action)
        {
            driverScope.RetryUntilTimeout(action);
        }

        public TResult RetryUntilTimeout<TResult>(Func<TResult> function)
        {
            return driverScope.RetryUntilTimeout(function);
        }

        public void RetryUntilTimeout(DriverAction driverAction)
        {
            driverScope.RetryUntilTimeout(driverAction);
        }

        public T Query<T>(Func<T> query, T expecting)
        {
            return driverScope.Query(query, expecting);
        }

        public T Query<T>(Query<T> query)
        {
            return driverScope.Query(query);
        }

        public void TryUntil(Action tryThis, Func<bool> until, TimeSpan waitBeforeRetry)
        {
            driverScope.TryUntil(tryThis, until, waitBeforeRetry);
        }

        public void TryUntil(DriverAction tryThis, Predicate until, TimeSpan waitBeforeRetry)
        {
            driverScope.TryUntil(tryThis, until, waitBeforeRetry);
        }

        public State FindState(params State[] states)
        {
            return driverScope.FindState(states);
        }

        public Session ConsideringInvisibleElements()
        {
            driverScope.ConsideringInvisibleElements();
            return this;
        }

        public Session ConsideringOnlyVisibleElements()
        {
            driverScope.ConsideringOnlyVisibleElements();
            return this;
        }

        public Session WithTimeout(TimeSpan timeout)
        {
            driverScope.WithTimeout(timeout);
            return this;
        }
    }
}
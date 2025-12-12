try {
	var piwikTracker = Piwik.getTracker(pkBaseURL + "piwik.php", 35);
	piwikTracker.trackPageView();
	piwikTracker.enableLinkTracking();
	var piwikTracker2 = Piwik.getTracker(pkBaseURL + "piwik.php", 30);
	piwikTracker2.trackPageView();
	piwikTracker2.enableLinkTracking();
} catch( err ) {}

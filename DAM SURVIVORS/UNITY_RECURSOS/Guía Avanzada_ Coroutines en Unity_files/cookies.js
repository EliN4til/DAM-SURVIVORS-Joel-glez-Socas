var settings = {"useSecureCookies":true,
                "cookiePath":null,
                "cookieDomain":null,
                "cookieSameSite":"ES"};

document.addEventListener('DOMContentLoaded', function() {
        window.MatomoConsent.init(settings.useSecureCookies, settings.cookiePath, settings.cookieDomain, settings.cookieSameSite); });

window.MatomoConsent = {
        cookiesDisabled: (!navigator || !navigator.cookieEnabled),
        CONSENT_COOKIE_NAME: 'mtm_consent', CONSENT_REMOVED_COOKIE_NAME: 'mtm_consent_removed',
        cookieIsSecure: false, useSecureCookies: true, cookiePath: '', cookieDomain: '', cookieSameSite: settings.cookieSameSite,
        init: function(useSecureCookies, cookiePath, cookieDomain, cookieSameSite) {
                this.useSecureCookies = useSecureCookies; this.cookiePath = cookiePath;
                this.cookieDomain = cookieDomain; this.cookieSameSite = cookieSameSite;
                if(useSecureCookies && location.protocol !== 'https:') {
                        console.log('Error with setting useSecureCookies: You cannot use this option on http.');
                } else {
                        this.cookieIsSecure = useSecureCookies;
                }
        },
        consentGiven: function() {
                this.setCookie(this.CONSENT_REMOVED_COOKIE_NAME, '', -129600000);
                this.setCookie(this.CONSENT_COOKIE_NAME, new Date().getTime(), 0);
        },
        consentRevoked: function() {
                this.setCookie(this.CONSENT_COOKIE_NAME, '', -129600000);
                this.setCookie(this.CONSENT_REMOVED_COOKIE_NAME, new Date().getTime(), 0);
        },
        getCookie: function(cookieName) {
                var cookiePattern = new RegExp('(^|;)[ ]*' + cookieName + '=([^;]*)'), cookieMatch = cookiePattern.exec(document.cookie);
                return cookieMatch ? window.decodeURIComponent(cookieMatch[2]) : 0;
        },
        setCookie: function(cookieName, value, msToExpire) {
                var expiryDate = new Date();
                expiryDate.setTime((new Date().getTime()) + msToExpire);
                document.cookie = cookieName + '=' + window.encodeURIComponent(value) +
                        (msToExpire ? ';expires=' + expiryDate.toGMTString() : '') +
                        ';path=' + (this.cookiePath || '/') +
                        (this.cookieDomain ? ';domain=' + this.cookieDomain : '') +
                        (this.cookieIsSecure ? ';secure' : '') +
                        ';SameSite=' + this.cookieSameSite;
                if ((!msToExpire || msToExpire >= 0) && this.getCookie(cookieName) !== String(value)) {
                        console.log('There was an error setting cookie `' + cookieName + '`. Please check domain and path.');
                }
        }
};


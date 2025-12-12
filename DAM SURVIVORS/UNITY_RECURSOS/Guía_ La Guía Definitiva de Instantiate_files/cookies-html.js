// Función para crear el banner de cookies
function createCookieBanner() {
	var _paq = window._paq = window._paq || [];
	const mtmConsentCookie = window.MatomoConsent.getCookie("mtm_consent");
    const mtmConsentRemovedCookie = window.MatomoConsent.getCookie("mtm_consent_removed");

    if (mtmConsentCookie === 0 && mtmConsentRemovedCookie === 0) {
		
		const wrapper = document.createElement('div');
		wrapper.classList.add('bannerwrapper');
		
		const logo = document.createElement('img');
		logo.src = 'https://www3.gobiernodecanarias.org/educacion/cau_ce/cookies/logo.png';
		logo.id = 'bannerlogo';
		
		const h2 = document.createElement('h2');
		h2.textContent = 'Consentimiento de Cookies';
		
		const header = document.createElement('header');
		header.classList.add('bannerheader');
		header.appendChild(logo);
		header.appendChild(h2);

		const matomoOptOut = document.createElement('div');
		matomoOptOut.id = 'matomo-opt-out';

		const p1 = document.createElement('p');
		p1.textContent = 'Utilizamos tecnologías como las cookies para almacenar y/o acceder a la información del dispositivo. Lo hacemos para mejorar la experiencia de navegación y con fines estadísticos. No consentir o retirar el consentimiento, puede afectar negativamente a ciertas características y funciones.';
		
		const p2 = document.createElement('p');
		p2.textContent = 'Para conocer más acerca de las cookies puede consultar nuestra ';
		
		const politicalink = document.createElement('a');
		politicalink.href = 'http://www.gobiernodecanarias.org/principal/politica-de-cookies/index.html';
		politicalink.textContent = 'Política de cookies';
		p2.appendChild(politicalink);
		
		matomoOptOut.appendChild(p1);
		matomoOptOut.appendChild(p2);

		const buttons = document.createElement('div');
		buttons.classList.add('bannerbuttons');
		
		const acceptBtn = document.createElement('button');
		acceptBtn.classList.add('button');
		acceptBtn.id = 'acceptBtn';
		acceptBtn.textContent = 'Aceptar';

		// Lógica para aceptar las cookies
		acceptBtn.onclick = () => {
				_paq.push(['trackEvent', 'forgetUserOptOut']);
				window.MatomoConsent.consentGiven();
				wrapper.classList.add('hidden');
		};

		const declineBtn = document.createElement('button');
		declineBtn.classList.add('button');
		declineBtn.id = 'declineBtn';
		declineBtn.textContent = 'Rechazar';

		// Lógica para rechazar las cookies
		declineBtn.onclick = () => {
				_paq.push(['optUserOut']);
				window.MatomoConsent.consentRevoked();
				wrapper.classList.add('hidden');
		};
		buttons.appendChild(acceptBtn);
		buttons.appendChild(declineBtn);

		wrapper.appendChild(header);
		wrapper.appendChild(matomoOptOut);
		wrapper.appendChild(buttons);

		document.body.appendChild(wrapper);
	}
}

window.onload = createCookieBanner;



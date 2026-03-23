// Apply saved theme immediately (before page renders)
(function() {
  const saved = localStorage.getItem('theme');
  if (saved === 'light') {
    document.body.className = 'light-mode';
  }
})();

// Shared site utilities — available globally for sub-page scripts
const SiteIncludes = (function() {

  /**
   * Fetches an HTML snippet via AJAX and injects it into a specified DOM element.
   * This is used to dynamically load reusable site components (like headers and footers) 
   * into static HTML files, preventing code duplication across the site.
   *
   * @param {string} url - The URL of the HTML partial to retrieve.
   * @param {string} targetId - The ID of the target element to inject the HTML into.
   * @param {function} [callback] - Optional callback function to run after insertion.
   */
  function loadPartial(url, targetId, callback) {
    const xhr = new XMLHttpRequest();
    xhr.open('GET', url, true);
    xhr.onreadystatechange = function() {
      if (xhr.readyState === 4 && xhr.status === 200) {
        document.getElementById(targetId).innerHTML = xhr.responseText;
        if (callback) callback();
      }
    };
    xhr.send();
  }

  /**
   * Adjusts 'href' and 'src' attributes within a container by prepending '../'.
   * This is necessary for shared components that contain root-relative links/images 
   * but are dynamically loaded into sub-pages (e.g., inside '/calculator-pages/').
   * It ensures paths correctly point up one directory level to the root assets.
   *
   * @param {string} containerSelector - CSS selector for the container to process.
   */
  function fixRelativeLinks(containerSelector) {
    const links = document.querySelectorAll(containerSelector + ' a');
    for (let i = 0; i < links.length; i++) {
      const href = links[i].getAttribute('href');
      if (href && href.indexOf('://') === -1 && href.indexOf('../') === -1 && href.indexOf('#') !== 0 && href.indexOf('mailto:') !== 0) {
        links[i].setAttribute('href', '../' + href);
      }
    }
    const imgs = document.querySelectorAll(containerSelector + ' img');
    for (let i = 0; i < imgs.length; i++) {
      const src = imgs[i].getAttribute('src');
      if (src && src.indexOf('://') === -1 && src.indexOf('../') === -1 && src.indexOf('data:') === -1) {
        imgs[i].setAttribute('src', '../' + src);
      }
    }
  }

  /**
   * Creates and appends a theme toggle button to the document body.
   * This provides the user interface for switching between light and dark modes.
   * The user's preference is saved to localStorage to persist across pages and visits.
   */
  function createThemeToggle() {
    const btn = document.createElement('button');
    btn.id = 'theme-toggle';
    btn.textContent = document.body.className === 'light-mode' ? '[ Dark Mode ]' : '[ Light Mode ]';
    btn.onclick = function() {
      if (document.body.className === 'light-mode') {
        document.body.className = '';
        localStorage.setItem('theme', 'dark');
        btn.textContent = '[ Light Mode ]';
      } else {
        document.body.className = 'light-mode';
        localStorage.setItem('theme', 'light');
        btn.textContent = '[ Dark Mode ]';
      }
    };
    document.body.appendChild(btn);
  }

  return {
    loadPartial: loadPartial,
    fixRelativeLinks: fixRelativeLinks,
    createThemeToggle: createThemeToggle
  };
})();

// Auto-initialize for root pages
// Sub-page scripts like calc-includes.js should set window._skipAutoInit = true
// before loading this file so they can handle initialization themselves.
if (!window._skipAutoInit) {
  (function() {
    // Load header, then highlight the active nav link
    SiteIncludes.loadPartial('header.html', 'header-container', function() {
      const page = window.location.pathname.split('/').pop() || 'index.html';
      const navMap = {
        'index.html': 'nav-home',
        'about.html': 'nav-about',
        'yugoslavia.html': 'nav-yugoslavia-dev',
        'photos.html': 'nav-photos',
        'links.html': 'nav-links',
        'calculator-museum.html': 'nav-calculator-museum',
        'ai-topic.html': 'nav-ai-topic',
        'contact.html': 'nav-contact'
      };
      const activeId = navMap[page];
      if (activeId) {
        const el = document.getElementById(activeId);
        if (el) el.className = 'active';
      }
    });

    // Load footer
    SiteIncludes.loadPartial('footer.html', 'footer-container');

    // Load main sidebar if container exists on this page
    if (document.getElementById('main-sidebar-container')) {
      SiteIncludes.loadPartial('sidebar.html', 'main-sidebar-container');
    }

    // Load separate calculator sidebar if container exists on root page
    if (document.getElementById('calc-sidebar-container')) {
      SiteIncludes.loadPartial('calculator-pages/calc-sidebar.html', 'calc-sidebar-container', function() {
        const links = document.querySelectorAll('#calc-sidebar-container a');
        for (let i = 0; i < links.length; i++) {
          const href = links[i].getAttribute('href');
          if (href && href.indexOf('://') === -1 && href.indexOf('#') !== 0 && href.indexOf('mailto:') !== 0) {
            if (href.indexOf('../') === 0) {
              links[i].setAttribute('href', href.substring(3));
            } else {
              links[i].setAttribute('href', 'calculator-pages/' + href);
            }
          }
        }
      });
    }

    // Load calculator museum license if container exists on this page
    if (document.getElementById('calc-license-container')) {
      SiteIncludes.loadPartial('calculator-pages/calc-license.html', 'calc-license-container');
    }

    // Create theme toggle
    SiteIncludes.createThemeToggle();
  })();
}

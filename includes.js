// Apply saved theme immediately (before page renders)
(function() {
  var saved = localStorage.getItem('theme');
  if (saved === 'light') {
    document.body.className = 'light-mode';
  }
})();

// Shared site utilities — available globally for sub-page scripts
var SiteIncludes = (function() {

  function loadPartial(url, targetId, callback) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', url, true);
    xhr.onreadystatechange = function() {
      if (xhr.readyState === 4 && xhr.status === 200) {
        document.getElementById(targetId).innerHTML = xhr.responseText;
        if (callback) callback();
      }
    };
    xhr.send();
  }

  // Fix relative links and image sources inside a container to point up one directory
  function fixRelativeLinks(containerSelector) {
    var links = document.querySelectorAll(containerSelector + ' a');
    for (var i = 0; i < links.length; i++) {
      var href = links[i].getAttribute('href');
      if (href && href.indexOf('://') === -1 && href.indexOf('../') === -1 && href.indexOf('#') !== 0 && href.indexOf('mailto:') !== 0) {
        links[i].setAttribute('href', '../' + href);
      }
    }
    var imgs = document.querySelectorAll(containerSelector + ' img');
    for (var i = 0; i < imgs.length; i++) {
      var src = imgs[i].getAttribute('src');
      if (src && src.indexOf('://') === -1 && src.indexOf('../') === -1 && src.indexOf('data:') === -1) {
        imgs[i].setAttribute('src', '../' + src);
      }
    }
  }

  function createThemeToggle() {
    var btn = document.createElement('button');
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
      var page = window.location.pathname.split('/').pop() || 'index.html';
      var navMap = {
        'index.html': 'nav-home',
        'about.html': 'nav-about',
        'yugoslavia.html': 'nav-yugoslavia-dev',
        'photos.html': 'nav-photos',
        'links.html': 'nav-links',
        'calculator-museum.html': 'nav-calculator-museum',
        'ai-topic.html': 'nav-ai-topic',
        'contact.html': 'nav-contact'
      };
      var activeId = navMap[page];
      if (activeId) {
        var el = document.getElementById(activeId);
        if (el) el.className = 'active';
      }
    });

    // Load footer
    SiteIncludes.loadPartial('footer.html', 'footer-container');

    // Load calculator museum license if container exists on this page
    if (document.getElementById('calc-license-container')) {
      SiteIncludes.loadPartial('calculator-pages/calc-license.html', 'calc-license-container');
    }

    // Create theme toggle
    SiteIncludes.createThemeToggle();
  })();
}

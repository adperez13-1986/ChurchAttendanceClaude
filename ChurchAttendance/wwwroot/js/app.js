// Custom Modal Functions
function openModal() {
    var overlay = document.getElementById('member-modal-overlay');
    if (overlay) {
        overlay.style.display = 'flex';
        document.body.style.overflow = 'hidden'; // Prevent background scrolling
    }
}

function closeModal() {
    var overlay = document.getElementById('member-modal-overlay');
    if (overlay) {
        overlay.style.display = 'none';
        document.body.style.overflow = ''; // Restore background scrolling
    }
}

// Close modal on successful form submission (htmx event)
document.addEventListener('htmx:afterOnLoad', function(event) {
    // If the request was to /members (POST or PUT) and succeeded, close the modal
    if (event.detail.xhr && event.detail.xhr.status === 200) {
        var url = event.detail.pathInfo.requestPath;
        if (url.startsWith('/members') && (event.detail.verb === 'POST' || event.detail.verb === 'PUT')) {
            closeModal();
        }
    }

    // Update attendance counter after attendance list loads
    if (event.detail.xhr && event.detail.pathInfo.requestPath === '/attendance/list') {
        updateAttendanceCount();
    }

    // Reset member filters after members table is swapped (add/edit/deactivate)
    if (event.detail.xhr && event.detail.pathInfo.requestPath.startsWith('/members')) {
        var ageSelect = document.getElementById('member-age-group-filter');
        var nameInput = document.getElementById('member-name-filter');
        if (ageSelect) ageSelect.value = '';
        if (nameInput) nameInput.value = '';
    }
});

// Auto-infer service type from selected date
function updateServiceType() {
    var dateInput = document.getElementById('date');
    var serviceInput = document.getElementById('serviceType');
    if (!dateInput || !serviceInput || !dateInput.value) return;

    var d = new Date(dateInput.value + 'T00:00:00');
    var day = d.getDay();

    // Sunday (0) → SundayService, Friday (5) → PrayerMeeting, Others → SundayService (default)
    if (day === 5) {
        serviceInput.value = 'PrayerMeeting';
    } else {
        serviceInput.value = 'SundayService';
    }
}

// Initialize service type on page load
document.addEventListener('DOMContentLoaded', function() {
    updateServiceType();
});

// Update service type after HTMX loads/swaps content (for attendance form)
document.addEventListener('htmx:afterSwap', function(event) {
    updateServiceType();
});

// CRITICAL: Update service type BEFORE HTMX sends the request
document.addEventListener('htmx:configRequest', function(event) {
    // If this is a request that includes the date/serviceType parameters
    if (event.detail.path === '/attendance/list' || event.detail.path === '/attendance') {
        updateServiceType();
    }
});

// Update service type when date changes
document.addEventListener('change', function (e) {
    if (e.target && e.target.id === 'date') {
        updateServiceType();
    }
});

// Update attendance counter display
function updateAttendanceCount() {
    var counterDiv = document.getElementById('attendance-count');
    if (!counterDiv) return;

    var allCheckboxes = document.querySelectorAll('input[name="memberIds"]');
    var totalChecked = 0;
    var visibleChecked = 0;

    allCheckboxes.forEach(function(cb) {
        var row = cb.closest('tr[data-age-group]');
        if (cb.checked) {
            totalChecked++;
            if (row && row.style.display !== 'none') {
                visibleChecked++;
            }
        }
    });

    var ageSelect = document.getElementById('age-group-filter');
    var isFiltered = ageSelect && ageSelect.value !== '';

    if (isFiltered) {
        counterDiv.innerHTML = '<strong>' + visibleChecked + ' present</strong> <span class="secondary-count">(' + totalChecked + ' total)</span>';
    } else {
        counterDiv.innerHTML = '<strong>' + totalChecked + ' present</strong>';
    }
}

// Apply name and age group filters on the members page
function filterMemberRows() {
    var ageSelect = document.getElementById('member-age-group-filter');
    var nameInput = document.getElementById('member-name-filter');
    var ageValue = ageSelect ? ageSelect.value : '';
    var nameValue = nameInput ? nameInput.value.toLowerCase() : '';
    var rows = document.querySelectorAll('#members-table tr[data-age-group]');
    rows.forEach(function (row) {
        var matchesAge = !ageValue || row.getAttribute('data-age-group') === ageValue;
        var matchesName = !nameValue || row.getAttribute('data-name').indexOf(nameValue) !== -1;
        row.style.display = (matchesAge && matchesName) ? '' : 'none';
    });
}

// Apply both name and age group filters together
function filterAttendanceRows() {
    var ageSelect = document.getElementById('age-group-filter');
    var nameInput = document.getElementById('name-filter');
    var ageValue = ageSelect ? ageSelect.value : '';
    var nameValue = nameInput ? nameInput.value.toLowerCase() : '';
    var rows = document.querySelectorAll('tr[data-age-group]');
    rows.forEach(function (row) {
        var matchesAge = !ageValue || row.getAttribute('data-age-group') === ageValue;
        var matchesName = !nameValue || row.getAttribute('data-name').indexOf(nameValue) !== -1;
        row.style.display = (matchesAge && matchesName) ? '' : 'none';
    });
    updateAttendanceCount();
}

// Auto-save attendance on checkbox change
function autoSaveAttendance() {
    var form = document.querySelector('form[hx-post="/attendance"]');
    if (!form) return;
    var formData = new FormData(form);
    var status = document.getElementById('auto-save-status');
    if (status) status.innerHTML = '<em>Saving...</em>';
    fetch('/attendance/auto-save', { method: 'POST', body: formData })
        .then(function (r) { return r.text(); })
        .then(function (html) { if (status) status.innerHTML = html; })
        .catch(function () { if (status) status.innerHTML = '<span class="status-msg error" style="padding:0.3rem 0.6rem;font-size:0.85rem">Save failed</span>'; });
}

// Select-all checkbox for attendance (only toggles visible rows)
document.addEventListener('change', function (e) {
    if (e.target && e.target.id === 'select-all') {
        var rows = document.querySelectorAll('tr[data-age-group]');
        rows.forEach(function (row) {
            if (row.style.display !== 'none') {
                var cb = row.querySelector('input[name="memberIds"]');
                if (cb) cb.checked = e.target.checked;
            }
        });
        updateAttendanceCount();
        autoSaveAttendance();
    }

    // Auto-save on individual checkbox toggle
    if (e.target && e.target.name === 'memberIds') {
        updateAttendanceCount();
        autoSaveAttendance();
    }

    // Age group filter (attendance page)
    if (e.target && e.target.id === 'age-group-filter') {
        filterAttendanceRows();
    }

    // Age group filter (members page)
    if (e.target && e.target.id === 'member-age-group-filter') {
        filterMemberRows();
    }
});

// Name filter (fires on every keystroke)
document.addEventListener('input', function (e) {
    if (e.target && e.target.id === 'name-filter') {
        filterAttendanceRows();
    }

    // Name filter (members page)
    if (e.target && e.target.id === 'member-name-filter') {
        filterMemberRows();
    }

    // Sync report date inputs with hidden form fields
    if (e.target.id === 'startDate') {
        document.querySelectorAll('.report-start-date').forEach(function (el) {
            el.value = e.target.value;
        });
    }
    if (e.target.id === 'endDate') {
        document.querySelectorAll('.report-end-date').forEach(function (el) {
            el.value = e.target.value;
        });
    }
});

// PDF Sharing functionality
function getPdfFileName() {
    var startDate = document.getElementById('startDate').value;
    var endDate = document.getElementById('endDate').value;
    return 'Church-Attendance-Report-' + startDate + '-to-' + endDate + '.pdf';
}

function sharePdf() {
    var statusDiv = document.getElementById('report-status');

    // Check if Web Share API is supported
    if (!navigator.share || !navigator.canShare) {
        statusDiv.innerHTML = '<p class="status-msg error">Sharing is not supported on this device. Use "Download PDF" instead.</p>';
        return;
    }

    statusDiv.innerHTML = '<p class="status-msg">Preparing PDF for sharing...</p>';

    var fileName = getPdfFileName();
    var startDate = document.getElementById('startDate').value;
    var endDate = document.getElementById('endDate').value;

    // Fetch the PDF as a blob
    var formData = new FormData();
    formData.append('startDate', startDate);
    formData.append('endDate', endDate);

    fetch('/reports/export', {
        method: 'POST',
        body: formData
    })
    .then(function(response) {
        if (!response.ok) {
            throw new Error('Failed to generate PDF');
        }
        return response.blob();
    })
    .then(function(blob) {
        // Create a File object from the blob
        var file = new File([blob], fileName, { type: 'application/pdf' });

        // Check if we can share files
        if (navigator.canShare && navigator.canShare({ files: [file] })) {
            return navigator.share({
                files: [file],
                title: 'Church Attendance Report',
                text: 'Attendance report from ' + startDate + ' to ' + endDate
            });
        } else {
            // Fall back to sharing URL if file sharing isn't supported
            throw new Error('File sharing not supported');
        }
    })
    .then(function() {
        statusDiv.innerHTML = '<p class="status-msg success">PDF shared successfully!</p>';
    })
    .catch(function(error) {
        if (error.name === 'AbortError') {
            // User cancelled the share
            statusDiv.innerHTML = '';
        } else {
            // Fall back to download
            statusDiv.innerHTML = '<p class="status-msg error">Sharing failed. Downloading instead...</p>';
            document.getElementById('export-pdf-form').submit();
        }
    });
}

// PDF Sharing functionality for Attendance tab
function getAttendancePdfFileName() {
    var form = document.querySelector('form[hx-post="/attendance"]');
    if (!form) return 'Church-Attendance.pdf';

    var dateInput = form.querySelector('input[name="date"]');
    var serviceTypeInput = form.querySelector('input[name="serviceType"]');

    var date = dateInput ? dateInput.value : '';
    var serviceType = serviceTypeInput ? serviceTypeInput.value : 'SundayService';
    var serviceLabel = serviceType === 'PrayerMeeting' ? 'Prayer-Meeting' : 'Sunday-Service';

    return 'Church-Attendance-' + serviceLabel + '-' + date + '.pdf';
}

function shareAttendancePdf() {
    var statusDiv = document.getElementById('attendance-pdf-status');
    if (!statusDiv) {
        statusDiv = document.createElement('div');
        statusDiv.id = 'attendance-pdf-status';
        var formParent = document.querySelector('form[hx-post="/attendance"]');
        if (formParent) {
            formParent.appendChild(statusDiv);
        }
    }

    // Check if Web Share API is supported
    if (!navigator.share || !navigator.canShare) {
        statusDiv.innerHTML = '<p class="status-msg error">Sharing is not supported on this device. Use "View Summary" to see the report.</p>';
        return;
    }

    statusDiv.innerHTML = '<p class="status-msg">Preparing PDF for sharing...</p>';

    var fileName = getAttendancePdfFileName();
    var form = document.querySelector('form[hx-post="/attendance"]');
    if (!form) {
        statusDiv.innerHTML = '<p class="status-msg error">Form not found.</p>';
        return;
    }

    var dateInput = form.querySelector('input[name="date"]');
    var serviceTypeInput = form.querySelector('input[name="serviceType"]');

    var date = dateInput ? dateInput.value : '';
    var serviceType = serviceTypeInput ? serviceTypeInput.value : 'SundayService';
    var serviceLabel = serviceType === 'PrayerMeeting' ? 'Prayer Meeting' : 'Sunday Service';

    // Fetch the PDF as a blob
    var formData = new FormData();
    formData.append('date', date);
    formData.append('serviceType', serviceType);

    fetch('/attendance/export-pdf', {
        method: 'POST',
        body: formData
    })
    .then(function(response) {
        if (!response.ok) {
            throw new Error('Failed to generate PDF');
        }
        return response.blob();
    })
    .then(function(blob) {
        // Create a File object from the blob
        var file = new File([blob], fileName, { type: 'application/pdf' });

        // Check if we can share files
        if (navigator.canShare && navigator.canShare({ files: [file] })) {
            return navigator.share({
                files: [file],
                title: 'Church Attendance - ' + serviceLabel,
                text: 'Attendance for ' + serviceLabel + ' on ' + date
            });
        } else {
            // Fall back to download if file sharing isn't supported
            throw new Error('File sharing not supported');
        }
    })
    .then(function() {
        statusDiv.innerHTML = '<p class="status-msg success">PDF shared successfully!</p>';
    })
    .catch(function(error) {
        if (error.name === 'AbortError') {
            // User cancelled the share
            statusDiv.innerHTML = '';
        } else {
            // Fall back to download
            statusDiv.innerHTML = '<p class="status-msg error">Sharing failed. Try using "View Summary" instead.</p>';
        }
    });
}

// Intercept Export PDF button to use share API on mobile if available
document.addEventListener('DOMContentLoaded', function() {
    var exportBtn = document.getElementById('export-pdf-btn');
    var exportForm = document.getElementById('export-pdf-form');
    var shareBtn = document.getElementById('share-pdf-btn');

    if (exportBtn && exportForm) {
        exportBtn.addEventListener('click', function(e) {
            // Check if we're on a mobile device and Web Share API is available
            var isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);

            if (isMobile && navigator.share && navigator.canShare) {
                e.preventDefault();
                sharePdf();
            }
            // Otherwise, let the form submit normally for download
        });
    }

    // Share button is always visible - sharePdf() will handle unsupported devices
});

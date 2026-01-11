// API Configuration - points to backend
const API_BASE_URL = window.location.origin + '/api';

// State management
let authToken = localStorage.getItem('authToken');
let currentUser = JSON.parse(localStorage.getItem('currentUser') || 'null');

// Initialize the app
document.addEventListener('DOMContentLoaded', () => {
    if (authToken && currentUser) {
        showMainContent();
        loadIssues();
        loadTasks();
        if (currentUser.role === 'Staff') {
            loadDashboard();
        }
    } else {
        showLogin();
    }

    setupEventListeners();
});

function setupEventListeners() {
    // Login form
    document.getElementById('login-form').addEventListener('submit', handleLogin);
    
    // Logout button
    document.getElementById('logout-btn').addEventListener('click', handleLogout);
    
    // Tab buttons
    document.querySelectorAll('.tab-btn').forEach(btn => {
        btn.addEventListener('click', (e) => {
            const tabName = e.target.dataset.tab;
            switchTab(tabName);
        });
    });
    
    // Create issue button
    document.getElementById('create-issue-btn').addEventListener('click', () => {
        document.getElementById('create-issue-form').style.display = 'block';
    });
    
    // Cancel issue button
    document.getElementById('cancel-issue-btn').addEventListener('click', () => {
        document.getElementById('create-issue-form').style.display = 'none';
        document.getElementById('new-issue-form').reset();
    });
    
    // New issue form
    document.getElementById('new-issue-form').addEventListener('submit', handleCreateIssue);
}

async function handleLogin(e) {
    e.preventDefault();
    
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;
    const errorDiv = document.getElementById('login-error');
    
    try {
        const response = await fetch(`${API_BASE_URL}/auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email, password })
        });
        
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Login failed');
        }
        
        const data = await response.json();
        authToken = data.token;
        currentUser = {
            userId: data.userId,
            username: data.username,
            role: data.role
        };
        
        localStorage.setItem('authToken', authToken);
        localStorage.setItem('currentUser', JSON.stringify(currentUser));
        
        showMainContent();
        loadIssues();
        loadTasks();
        if (currentUser.role === 'Staff') {
            loadDashboard();
        }
    } catch (error) {
        errorDiv.textContent = error.message;
    }
}

function handleLogout() {
    authToken = null;
    currentUser = null;
    localStorage.removeItem('authToken');
    localStorage.removeItem('currentUser');
    showLogin();
}

function showLogin() {
    document.getElementById('login-section').style.display = 'block';
    document.getElementById('main-content').style.display = 'none';
    document.getElementById('user-info').style.display = 'none';
}

function showMainContent() {
    document.getElementById('login-section').style.display = 'none';
    document.getElementById('main-content').style.display = 'block';
    document.getElementById('user-info').style.display = 'flex';
    document.getElementById('username-display').textContent = `Welcome, ${currentUser.username} (${currentUser.role})`;
    
    // Show/hide elements based on role
    if (currentUser.role === 'Student') {
        document.getElementById('create-issue-btn').style.display = 'block';
    }
    
    if (currentUser.role === 'Staff') {
        document.querySelector('[data-tab="dashboard"]').style.display = 'block';
    }
}

function switchTab(tabName) {
    // Update tab buttons
    document.querySelectorAll('.tab-btn').forEach(btn => {
        btn.classList.remove('active');
    });
    document.querySelector(`[data-tab="${tabName}"]`).classList.add('active');
    
    // Update tab content
    document.querySelectorAll('.tab-content').forEach(content => {
        content.classList.remove('active');
    });
    document.getElementById(`${tabName}-tab`).classList.add('active');
}

async function apiRequest(endpoint, options = {}) {
    const headers = {
        'Content-Type': 'application/json',
        ...options.headers
    };
    
    if (authToken) {
        headers['Authorization'] = `Bearer ${authToken}`;
    }
    
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
        ...options,
        headers
    });
    
    if (response.status === 401) {
        handleLogout();
        throw new Error('Unauthorized - please login again');
    }
    
    if (!response.ok) {
        const error = await response.text();
        throw new Error(error || 'Request failed');
    }
    
    return response.json();
}

async function loadIssues() {
    try {
        const issues = await apiRequest('/issues');
        displayIssues(issues);
    } catch (error) {
        console.error('Failed to load issues:', error);
        document.getElementById('issues-list').innerHTML = 
            `<p class="error">Failed to load issues: ${error.message}</p>`;
    }
}

function displayIssues(issues) {
    const container = document.getElementById('issues-list');
    
    if (issues.length === 0) {
        container.innerHTML = '<p>No issues found.</p>';
        return;
    }
    
    container.innerHTML = issues.map(issue => `
        <div class="card">
            <h4>Issue #${issue.id}</h4>
            <p><strong>Description:</strong> ${issue.description}</p>
            <p><strong>Shared Space:</strong> ${issue.sharedSpace}</p>
            <p><strong>Status:</strong> <span class="status ${issue.status.toLowerCase()}">${issue.status}</span></p>
            ${issue.assignedToUsername ? `<p><strong>Assigned To:</strong> ${issue.assignedToUsername}</p>` : ''}
            ${issue.photoProof ? `<p><strong>Photo Proof:</strong> <a href="${issue.photoProof}" target="_blank">View</a></p>` : ''}
            <p><strong>Created:</strong> ${new Date(issue.createdAt).toLocaleString()}</p>
        </div>
    `).join('');
}

async function handleCreateIssue(e) {
    e.preventDefault();
    
    const description = document.getElementById('issue-description').value;
    const sharedSpace = document.getElementById('issue-space').value;
    const photoProof = document.getElementById('issue-photo').value || null;
    
    try {
        await apiRequest('/issues', {
            method: 'POST',
            body: JSON.stringify({
                description,
                sharedSpace,
                photoProof
            })
        });
        
        document.getElementById('create-issue-form').style.display = 'none';
        document.getElementById('new-issue-form').reset();
        await loadIssues();
    } catch (error) {
        alert('Failed to create issue: ' + error.message);
    }
}

async function loadTasks() {
    try {
        const [cleaningTasks, garbageTasks] = await Promise.all([
            apiRequest('/tasks/cleaning'),
            apiRequest('/tasks/garbage')
        ]);
        
        displayTasks('cleaning-tasks-list', cleaningTasks);
        displayTasks('garbage-tasks-list', garbageTasks);
    } catch (error) {
        console.error('Failed to load tasks:', error);
    }
}

function displayTasks(containerId, tasks) {
    const container = document.getElementById(containerId);
    
    if (tasks.length === 0) {
        container.innerHTML = '<p>No tasks found.</p>';
        return;
    }
    
    container.innerHTML = tasks.map(task => `
        <div class="card">
            <h4>Task #${task.id}</h4>
            <p><strong>Description:</strong> ${task.description}</p>
            <p><strong>${task.sharedSpace ? 'Shared Space' : 'Location'}:</strong> ${task.sharedSpace || task.location}</p>
            <p><strong>Status:</strong> <span class="status ${task.status.toLowerCase()}">${task.status}</span></p>
            ${task.assignedUsername ? `<p><strong>Assigned To:</strong> ${task.assignedUsername}</p>` : ''}
            <p><strong>Due Date:</strong> ${new Date(task.dueDate).toLocaleDateString()}</p>
            ${task.status === 'Pending' && task.assignedUserId === currentUser.userId ? 
                `<button class="btn btn-success" onclick="completeTask('${containerId.includes('cleaning') ? 'cleaning' : 'garbage'}', ${task.id})">Complete</button>` : ''}
        </div>
    `).join('');
}

async function completeTask(taskType, taskId) {
    try {
        await apiRequest(`/tasks/${taskType}/${taskId}/complete`, {
            method: 'POST'
        });
        await loadTasks();
    } catch (error) {
        alert('Failed to complete task: ' + error.message);
    }
}

async function loadDashboard() {
    try {
        const stats = await apiRequest('/dashboard');
        displayDashboard(stats);
    } catch (error) {
        console.error('Failed to load dashboard:', error);
        document.getElementById('dashboard-stats').innerHTML = 
            `<p class="error">Failed to load dashboard: ${error.message}</p>`;
    }
}

function displayDashboard(stats) {
    const container = document.getElementById('dashboard-stats');
    
    container.innerHTML = `
        <div class="stat-card">
            <h3>${stats.openIssues}</h3>
            <p>Open Issues</p>
        </div>
        <div class="stat-card">
            <h3>${stats.inProgressIssues}</h3>
            <p>In Progress Issues</p>
        </div>
        <div class="stat-card">
            <h3>${stats.resolvedIssues}</h3>
            <p>Resolved Issues</p>
        </div>
        <div class="stat-card">
            <h3>${stats.closedIssues}</h3>
            <p>Closed Issues</p>
        </div>
        <div class="stat-card">
            <h3>${stats.overdueTasks}</h3>
            <p>Overdue Tasks</p>
        </div>
        <div class="stat-card">
            <h3>${stats.tasksDueToday}</h3>
            <p>Tasks Due Today</p>
        </div>
    `;
}

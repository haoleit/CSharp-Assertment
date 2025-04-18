-- Create the tasks table
CREATE TABLE IF NOT EXISTS tasks (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    status VARCHAR(20) NOT NULL CHECK (status IN ('to-do', 'in progress', 'completed')),
    due_date DATE
);

-- Insert some sample tasks
INSERT INTO tasks (title, description, status, due_date) VALUES
    ('Implement user registration', 'Create the user registration endpoint', 'to-do', '2025-04-24'),
    ('Implement task management', 'Create the task management endpoints', 'in progress', '2025-05-01'),
    ('Implement filtering and sorting', 'Create the filtering and sorting functionality', 'completed', '2025-05-08');

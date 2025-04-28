<template>
  <div id="app" class="container">
    <h1>TODO APPLICATION</h1>

     <!-- for selecting provider here given two provider A and B -->

    <div v-if="!selectedProvider">
      <button @click="selectProvider('A')" class="btn custom-grey m-2">Select Provider A</button>
      <button @click="selectProvider('B')" class="btn custom-grey m-2">Select Provider B</button>
    </div>


    <div v-if="selectedProvider && !isFormVisible && !isAddingTodo && !isUpdatingTodo">
      <h2>Provider {{ selectedProvider }}</h2>

      <button @click="showAddTodoForm" class="btn custom-grey mb-3">Add Todo</button>  <!--add todo button -->

      <div class="mb-3">
        <input v-model="searchQuery" @input="debouncedSearchTodo"  placeholder="Search Todo by Name" class="form-control" /> <!--search bar -->
      </div>


      <div v-if="showingAllTodos || searchQuery">
        <!-- This part will show search results-->
        <div v-if="searchQuery && searchedTodo">
          <h3>Todo Found</h3>
          <table class="table table-bordered">
            <thead>
              <tr>
                <th>Todo</th>
                <th>Description</th>
                <th>Due Date</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td>{{ searchedTodo.name }}</td>
                <td>{{ searchedTodo.description }}</td>
                <td>{{ formatDateTime(searchedTodo.dueDate) }}</td>
                <td>
                  <button @click="showUpdateTodoForm(searchedTodo)" class="btn custom-grey btn-sm m-1">Update</button>
                  <button @click="deleteTodoById(searchedTodo.id)" class="btn btn-danger btn-sm m-1">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>



        <!--  This part will results to track --> 
        <div v-else>
          <h3>All Todos</h3>
          <table class="table table-bordered">
            <thead>
              <tr>
                <th>Todo</th>
                <th>Description</th>
                <th>Due Date</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="todo in todos" :key="todo.id">
                <td>{{ todo.name }}</td>
                <td>{{ todo.description }}</td>
                <td>{{ formatDateTime(todo.dueDate) }}</td>
                <td>
                  <button @click="showUpdateTodoForm(todo)" class="btn custom-grey btn-sm m-1">Update</button>
                  <button @click="deleteTodoById(todo.id)" class="btn btn-danger btn-sm m-1">Delete</button>
                </td>
              </tr>
              <tr v-if="todos.length === 0">
                <td colspan="4" class="text-center">No Todos Available</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div v-if="message" class="alert" :class="messageClass">{{ message }}</div>


      <button @click="backToProviderSelection" class="btn custom-grey mt-3">Back</button>
    </div>

    <!-- adding todo form-->
    <div v-if="isAddingTodo">
      <h3>Add Todo to Provider {{ selectedProvider }}</h3>
      <input v-model="newTodo.name" placeholder="Todo" class="form-control my-2" required/>
      <input v-model="newTodo.description" placeholder="Description" class="form-control my-2" required/>
      <input v-model="newTodo.dueDate" type="datetime-local" class="form-control my-2" required/>
      <button @click="addTodo" class="btn custom-grey">Add Todo</button>
      <button @click="cancelForm" class="btn custom-grey ml-2">Back</button>

      <div v-if="addmessage" class="alert" :class="addmessageclass">{{ addmessage }}</div>
    </div>

    <!-- Updating todo form -->
    <div v-if="isUpdatingTodo">
      <h3>Update Todo for Provider {{ selectedProvider }}</h3>
      <input v-model="editTodoData.name" placeholder="Todo" class="form-control my-2" />
      <input v-model="editTodoData.description" placeholder="Description" class="form-control my-2" />
      <input v-model="editTodoData.dueDate" type="datetime-local" class="form-control my-2" />
      <button @click="updateTodo" class="btn custom-grey">Update Todo</button>
      <button @click="cancelForm" class="btn custom-grey ml-2">Back</button>

      <div v-if="updatemessage" class="alert" :class="updatemessageclass">{{ updatemessage }}</div>
    </div>
  </div>
</template>


<script>
import axios from "axios";

export default {
  data() {
    return {
      todos: [],
      newTodo: { name: "", description: "", dueDate: "" },
      editTodoData: { id: null, name: "", description: "", dueDate: "" },
      //form visisblity control
      isAddingTodo: false,
      isUpdatingTodo: false,
      isFormVisible: false,

      selectedProvider: null,
      searchQuery: "",
      searchedTodo: null,
      debounceTimer: null,
     
     // message alerts 
      message: "",
      messageClass: "",
      addmessage: "",
      addmessageclass: "",
      updatemessage: "",
      updatemessageclass: "",
      showingAllTodos: false,
    };
  },
  methods: {
    //this sets the slected provider and then after selection loads all todos related to provider
    selectProvider(provider) {
      this.selectedProvider = provider;
      this.showAllTodos();
    },

    showAddTodoForm() {
      this.isAddingTodo = true;
      this.isUpdatingTodo = false;
      this.isFormVisible = true;
      this.addmessage = "";
      this.addmessageclass = "";
    },

    showUpdateTodoForm(todo) {
      this.isUpdatingTodo = true;
      this.isAddingTodo = false;
      this.isFormVisible = true;
      this.editTodoData = { ...todo };
      this.updatemessage = "";
      this.updatemessageclass = "";
    },

    cancelForm() {
      this.isAddingTodo = false;
      this.isUpdatingTodo = false;
      this.isFormVisible = false;
      this.searchQuery = "";
      this.searchedTodo = null;
      this.showAllTodos();
    },

    //fetches all todos
    async showAllTodos() {
      try {
        const response = await axios.get(`http://localhost:5149/todo/${this.selectedProvider}/all`);
        this.todos = response.data;
        this.searchedTodo = null;
        this.showingAllTodos = true;
      } catch (error) {
        console.error("Error loading todos:", error);
      }
    },


    // debounced search per required requirement without crashing with mutliple API calls while searching
    async debouncedSearchTodo() {
      if (this.debounceTimer) clearTimeout(this.debounceTimer);

      this.debounceTimer = setTimeout(async () => {
        if (!this.searchQuery.trim()) {
          this.searchedTodo = null;
          await this.showAllTodos();
          return;
        }
        try {
          const response = await axios.get(
            `http://localhost:5149/todo/${this.selectedProvider}/search/${this.searchQuery}`
          );
          if (response.status === 200 && response.data) {
            this.searchedTodo = response.data;
            this.showingAllTodos = false;
          } else {
            this.searchedTodo = null;
            this.todos = [];
          }
        } catch (error) {
          this.searchedTodo = null;
          this.todos = [];
        }
      }, 1000);
    },

    async addTodo() {
      try {
        await axios.get(`http://localhost:5149/todo/${this.selectedProvider}/search/${this.newTodo.name}`);
        this.addmessage = "Todo with this name already exists!";
        this.addmessageclass = "alert-danger";
        return;
      } catch (error) {
        if (error.response && error.response.status === 404) {
          try {
            const response = await axios.post(
              `http://localhost:5149/todo/${this.selectedProvider}`,
              this.newTodo
            );
            this.todos.push(response.data);
            this.newTodo = { name: "", description: "", dueDate: "" };
            this.addmessage = `Todo "${response.data.name}" added successfully`;
            this.addmessageclass = "alert-success";
           
          } catch (e) {
            this.addmessage = "Error adding todo.";
            this.addmessageclass = "alert-danger";
          }
        }
      }
    },

    async updateTodo() {
      try {
        const response = await axios.get(
          `http://localhost:5149/todo/${this.selectedProvider}/search/${this.editTodoData.name}`
        );
        if (response.status === 200 && response.data && response.data.id !== this.editTodoData.id) {
          this.updatemessage = "Cannot update as name already exists!";
          this.updatemessageclass = "alert-danger";
          return;
        }
      } catch (error) {
        if (!(error.response && error.response.status === 404)) {
          this.updatemessage = "Error checking todo name.";
          this.updatemessageclass = "alert-danger";
          return;
        }
      }

      try {
        const response = await axios.put(
          `http://localhost:5149/todo/${this.selectedProvider}/${this.editTodoData.id}`,
          this.editTodoData
        );
        const index = this.todos.findIndex((t) => t.id === this.editTodoData.id);
        if (index !== -1) {
          this.todos[index] = response.data;
        }
        this.updatemessage = "Todo updated successfully";
        this.updatemessageclass = "alert-success";
        setTimeout(() => {
          this.cancelForm();
        }, 2000);
      } catch (error) {
        this.updatemessage = "Error updating todo.";
        this.updatemessageclass = "alert-danger";
      }
    },

    async deleteTodoById(id) {
      try {
        await axios.delete(`http://localhost:5149/todo/${this.selectedProvider}/${id}`);
        this.todos = this.todos.filter((t) => t.id !== id);
        this.searchQuery = "";
        this.searchedTodo = null;
        this.showAllTodos();
      } catch (error) {
        console.error("Error deleting todo:", error);
      }
    },


    // this is just to neatly display date
    formatDateTime(dateTimeString) {
      const date = new Date(dateTimeString);
      return date.toLocaleString();
    },

    // function for back option to go to provider
    backToProviderSelection() {
      this.selectedProvider = null;
      this.todos = [];
      this.searchedTodo = null;
      this.message = "";
      this.messageClass = "";
      this.isAddingTodo = false;
      this.isUpdatingTodo = false;
    },
  },
};
</script>

<style scoped>
input {
  margin: 5px;
}
button {
  margin-left: 10px;
}
h1 {
  color: #333;
}
#app {
  text-align: center;
}
.alert {
  margin-top: 10px;
  padding: 10px;
}
.alert-success {
  background-color: #d4edda;
  color: #155724;
}
.alert-danger {
  background-color: #f8d7da;
  color: #721c24;
}
.custom-grey {
  background-color: grey;
  color: white;
  border: none;
}
</style>

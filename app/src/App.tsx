import "./App.css";
import { RouteSectionProps } from "@solidjs/router";
import Header from "./shared/components/Header";
import Footer from "./shared/components/Footer";
import { Component } from "solid-js";

const App: Component<RouteSectionProps> = (props) => {
  return (
    <div class="app-container">
      <Header />
      <div class="app-content">{props.children}</div>
      <Footer />
    </div>
  );
};

export default App;
